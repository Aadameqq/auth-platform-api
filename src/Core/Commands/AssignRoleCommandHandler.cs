using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class AssignRoleCommandHandler(UnitOfWork uow) : CommandHandler<AssignRoleCommand>
{
    public async Task<Result> Handle(AssignRoleCommand cmd)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(cmd.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.AssignRole(cmd.Role, cmd.IssuerId);

        if (result is { IsFailure: true, Exception: CannotManageOwn<Role> or RoleAlreadyAssigned })
        {
            return result.Exception;
        }

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
