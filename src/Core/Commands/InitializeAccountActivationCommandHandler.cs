using Core.Commands.Commands;
using Core.Domain;
using Core.Ports;

namespace Core.Commands;

public class InitializeAccountActivationCommandHandler(
    UnitOfWork uow,
    ConfirmationService confirmationService
) : InitializeConfirmationCommandHandler<InitializeAccountActivationCommand>(confirmationService)
{
    protected override async Task<Result<Account>> Prepare(InitializeAccountActivationCommand cmd)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.Id));

        // TODO: logic

        return account;
    }
}
