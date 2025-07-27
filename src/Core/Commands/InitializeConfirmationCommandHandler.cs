using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class InitializeConfirmationCommandHandler(ConfirmationService confirmationService)
    : CommandHandler<InitializeConfirmationCommand>
{
    public async Task<Result> Handle(InitializeConfirmationCommand cmd, CancellationToken _)
    {
        var account = await cmd.Identity.GetOrFail();
        var result = await confirmationService.BeginConfirmation(account, cmd.Action);

        if (result is { IsFailure: true, Exception: TooManyAttempts })
        {
            return result.Exception;
        }

        return Result.Success();
    }
}
