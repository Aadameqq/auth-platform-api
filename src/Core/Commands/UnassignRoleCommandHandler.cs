using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class UnassignRoleCommandHandler(UnitOfWork uow)
{
    public async Task<Result> Execute(Guid issuerId, Guid accountId)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(accountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.RemoveRole(issuerId);

        if (result is { IsFailure: true, Exception: CannotManageOwn<Role> })
        {
            return result.Exception;
        }

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
