using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class ResetPasswordCommandHandler(
    PasswordHasher passwordHasher,
    UnitOfWork uow,
    EmailConfirmationProvider confirmationProvider
) : CommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(ResetPasswordCommand cmd, CancellationToken _)
    {
        var result = await confirmationProvider.Finish(
            new ConfirmationDto(
                cmd.ConfirmationId,
                ConfirmationMethod.Email,
                ConfirmableAction.PasswordReset
            ),
            cmd.Code
        );

        if (
            result is
            {
                IsFailure: true,
                Exception: NoSuch or Expired or ConfirmationMismatch or InvalidConfirmationCode
            }
        )
        {
            return result.Exception;
        }

        var confirmation = result.Value;

        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(confirmation.OwnerId));

        var passwordHash = passwordHasher.HashPassword(cmd.NewPassword);

        account.ResetPassword(passwordHash);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
