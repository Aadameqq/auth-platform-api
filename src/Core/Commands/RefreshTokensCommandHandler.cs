using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class RefreshTokensCommandHandler(
    UnitOfWork uow,
    DateTimeProvider dateTimeProvider,
    TokenService tokenService
)
{
    public async Task<Result<TokenPairOutput>> Execute(string token)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var payload = await tokenService.FetchRefreshTokenPayloadIfValid(token);

        if (payload is null)
        {
            return new InvalidToken();
        }

        var account = await accountsRepository.FindById(payload.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.RefreshSession(payload.ToRefreshToken(), dateTimeProvider.Now());

        if (result is { IsFailure: true, Exception: NoSuch<AuthSession> or InvalidToken })
        {
            await accountsRepository.Update(account);
            await uow.Flush();
            return result.Exception;
        }

        var pair = tokenService.CreateTokenPair(account, result.Value.SessionId, result.Value.Id);

        await accountsRepository.Update(account);
        await uow.Flush();
        return pair;
    }
}
