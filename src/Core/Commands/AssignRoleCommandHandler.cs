using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class AssignRoleCommandHandler(UnitOfWork uow)
{
    public async Task<Result> Execute(Guid issuerId, Guid accountId, Role role)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(accountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.AssignRole(role, issuerId);

        if (result is { IsFailure: true, Exception: CannotManageOwn<Role> or RoleAlreadyAssigned })
        {
            return result.Exception;
        }

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
