using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class ResetPasswordCommandHandler(
    PasswordHasher passwordHasher,
    UnitOfWork uow,
    ConfirmationService confirmationService
) : RequireConfirmationCommandHandler<ResetPasswordCommand>(confirmationService)
{
    protected override async Task<Result> HandleWithConfirmation(
        Account account,
        ResetPasswordCommand cmd
    )
    {
        var accountsRepository = uow.GetAccountsRepository();

        if (!account.HasPassword())
        {
            return new NoPassword();
        }

        var passwordHash = passwordHasher.HashPassword(cmd.NewPassword);

        account.ResetPassword(passwordHash);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
