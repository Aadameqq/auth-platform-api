using Core.Commands.Commands;
using Core.Domain;
using Core.Other;
using Core.Ports;

namespace Core.Commands;

public class ConfirmActionCommandHandler(UnitOfWork uow, ConfirmationServiceFactory serviceFactory)
    : CommandHandler<ConfirmActionCommand>
{
    public async Task<Result> Handle(ConfirmActionCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.AccountId));

        var service = serviceFactory.CreateInstance(cmd.Method);

        return await service.Confirm(cmd.Code, cmd.Action, account);
    }
}
