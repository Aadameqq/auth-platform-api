using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class CreateAccountCommandHandler(
    UnitOfWork uow,
    PasswordHasher passwordHasher,
    EmailConfirmationProvider confirmationProvider,
    SessionCreator sessionCreator
) : CommandHandler<CreateAccountCommand, CreatedAccountOutput>
{
    public async Task<Result<CreatedAccountOutput>> Handle(
        CreateAccountCommand cmd,
        CancellationToken _
    )
    {
        var accountsRepository = uow.GetAccountsRepository();
        var found = await accountsRepository.FindByEmail(cmd.Email);

        if (found != null)
        {
            return new AlreadyExists<Account>();
        }

        var hashedPassword = passwordHasher.HashPassword(cmd.PlainPassword);

        var account = new Account(cmd.UserName, cmd.Email, hashedPassword);

        var result = sessionCreator.CreateSession(account);

        await accountsRepository.Create(account);
        await uow.Flush();

        var beginResult = await confirmationProvider.Begin(
            ConfirmableAction.AccountActivation,
            account
        );

        if (beginResult is { IsFailure: true, Exception: TooManyAttempts })
        {
            return beginResult.Exception;
        }

        return new CreatedAccountOutput(
            result.Value.AccessToken,
            result.Value.RefreshToken,
            beginResult.Value.Id
        );
    }
}
