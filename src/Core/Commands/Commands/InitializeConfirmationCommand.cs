using Core.Domain;

namespace Core.Commands.Commands;

public record InitializeConfirmationCommand(Guid AccountId, ConfirmableAction Action) : Command
{
}
