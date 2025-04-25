using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class UnassignRoleCommandHandler(UnitOfWork uow) : CommandHandler<UnassignRoleCommand>
{
    public async Task<Result> Handle(UnassignRoleCommand cmd)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(cmd.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.RemoveRole(cmd.IssuerId);

        if (result is { IsFailure: true, Exception: CannotManageOwn<Role> })
        {
            return result.Exception;
        }

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
