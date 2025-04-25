using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class LogInCommandHandler(
    UnitOfWork uow,
    TokenService tokenService,
    PasswordVerifier passwordVerifier,
    DateTimeProvider dateTimeProvider
)
{
    public async Task<Result<TokenPairOutput>> Execute(string email, string password)
    {
        var accountsRepository = uow.GetAccountsRepository();

        var account = await accountsRepository.FindByEmail(email);
        if (account is null)
        {
            return new NoSuch<Account>();
        }

        if (!account.HasPassword())
        {
            return new NoPassword();
        }

        if (!passwordVerifier.Verify(password, account.Password))
        {
            return new InvalidCredentials();
        }

        var result = account.CreateSession(dateTimeProvider.Now());

        if (result is { IsFailure: true, Exception: AccountNotActivated })
        {
            return result.Exception;
        }

        var tokenPair = tokenService.CreateTokenPair(
            account,
            result.Value.SessionId,
            result.Value.Id
        );

        await accountsRepository.Update(account);
        await uow.Flush();

        return tokenPair;
    }
}
