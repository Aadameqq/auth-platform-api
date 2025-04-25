using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class ResetPasswordCommandHandler(
    PasswordHasher passwordHasher,
    UnitOfWork uow,
    PasswordResetCodesRepository passwordResetCodesRepository
) : CommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(ResetPasswordCommand cmd)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var accountId = await passwordResetCodesRepository.GetAccountIdAndRevokeCode(cmd.ResetCode);

        if (accountId is null)
        {
            return new NoSuch();
        }

        var account = await accountsRepository.FindById(accountId.Value);

        if (account is null)
        {
            return new NoSuch();
        }

        var passwordHash = passwordHasher.HashPassword(cmd.NewPassword);

        account.ResetPassword(passwordHash);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
