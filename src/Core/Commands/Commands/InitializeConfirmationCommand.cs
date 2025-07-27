using Core.Domain;
using Core.Ports;

namespace Core.Commands.Commands;

public record InitializeConfirmationCommand(Identity Identity, ConfirmableAction Action)
    : Command
{
}
