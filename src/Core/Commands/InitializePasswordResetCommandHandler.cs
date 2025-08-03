using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Other;
using Core.Ports;

namespace Core.Commands;

public class InitializePasswordResetCommandHandler(
    UnitOfWork uow,
    EmailConfirmationProvider confirmationProvider
) : CommandHandler<InitializePasswordResetCommand, Confirmation>
{
    public async Task<Result<Confirmation>> Handle(
        InitializePasswordResetCommand cmd,
        CancellationToken _
    )
    {
        var accountsRepository = uow.GetAccountsRepository();
        var found = await accountsRepository.FindByEmail(cmd.Email);

        if (found is null)
        {
            return new NoSuch<Account>();
        }

        if (!found.HasPassword())
        {
            return new NoPassword();
        }

        var beginResult = await confirmationProvider.Begin(ConfirmableAction.PasswordReset, found);

        if (beginResult is { IsFailure: true, Exception: TooManyAttempts })
        {
            return beginResult.Exception;
        }

        return beginResult.Value;
    }
}
