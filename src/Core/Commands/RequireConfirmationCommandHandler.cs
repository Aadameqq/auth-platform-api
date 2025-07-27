using Core.Commands.Commands;
using Core.Domain;
using Core.Ports;

namespace Core.Commands;

public abstract class RequireConfirmationCommandHandler<TCommand>(
    ConfirmationService confirmationService
) : CommandHandler<TCommand>
    where TCommand : RequireConfirmationCommand
{
    public async Task<Result> Handle(TCommand cmd, CancellationToken cancellationToken)
    {
        var result = await confirmationService.Confirm(cmd.Code, cmd.Action, cmd.AccountIdentity);

        if (result.IsFailure)
        {
            return result.Exception;
        }

        return await HandleWithConfirmation(result.Value, cmd);
    }

    protected abstract Task<Result> HandleWithConfirmation(Account account, TCommand command);
}
