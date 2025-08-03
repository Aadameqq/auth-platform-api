using Core.Commands.Commands;
using Core.Domain;
using Core.Ports;

namespace Core.Commands;

public class ActivateAccountCommandHandler(UnitOfWork uow) : CommandHandler<ActivateAccountCommand>
{
    public async Task<Result> Handle(ActivateAccountCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.Id));

        account.Activate();

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
