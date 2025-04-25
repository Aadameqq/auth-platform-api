using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class ActivateAccountCommandHandler(
    ActivationCodesRepository activationCodesRepository,
    UnitOfWork uow
)
{
    public async Task<Result> Execute(string code)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var userId = await activationCodesRepository.GetAccountIdAndRevokeCode(code);

        if (userId is null)
        {
            return new NoSuch();
        }

        var user = await accountsRepository.FindById(userId.Value);

        if (user is null || user.HasBeenActivated())
        {
            return new NoSuch<Account>();
        }

        user.Activate();

        await accountsRepository.Update(user);
        await uow.Flush();

        return Result.Success();
    }
}
