using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class ResetPasswordCommandHandler(
    PasswordHasher passwordHasher,
    UnitOfWork uow,
    PasswordResetCodesRepository passwordResetCodesRepository
)
{
    public async Task<Result> Execute(string resetCode, string newPassword)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var accountId = await passwordResetCodesRepository.GetAccountIdAndRevokeCode(resetCode);

        if (accountId is null)
        {
            return new NoSuch();
        }

        var account = await accountsRepository.FindById(accountId.Value);

        if (account is null)
        {
            return new NoSuch();
        }

        var passwordHash = passwordHasher.HashPassword(newPassword);

        account.ResetPassword(passwordHash);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
