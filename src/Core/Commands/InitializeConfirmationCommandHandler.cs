using Core.Commands.Commands;
using Core.Domain;
using Core.Ports;

namespace Core.Commands;

public class InitializeConfirmationCommandHandler(
    ConfirmationService confirmationService,
    UnitOfWork uow
) : CommandHandler<InitializeConfirmationCommand>
{
    public async Task<Result> Handle(InitializeConfirmationCommand cmd, CancellationToken _)
    {
        var accountRepository = uow.GetAccountsRepository();

        var found = await accountRepository.FindByIdOrFail(cmd.AccountId);

        await confirmationService.BeginConfirmation(found, cmd.Action);

        return Result.Success();
    }
}
