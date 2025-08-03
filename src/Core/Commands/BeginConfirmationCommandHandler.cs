using Core.Commands.Commands;
using Core.Domain;
using Core.Other;
using Core.Ports;

namespace Core.Commands;

public class BeginConfirmationCommandHandler(
    UnitOfWork uow,
    ConfirmationServiceFactory serviceFactory
) : CommandHandler<BeginConfirmationCommand>
{
    public async Task<Result> Handle(BeginConfirmationCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.AccountId));

        var service = serviceFactory.CreateInstance(cmd.Method);

        return await service.BeginConfirmation(account, cmd.Action);
    }
}
