using Core.Domain;
using Core.Ports;

namespace Core.Commands.Commands;

public record RequireConfirmationCommand<TOutput>(
    string Code,
    Identity AccountIdentity,
    ConfirmableAction Action
) : Command<TOutput> { }

public record RequireConfirmationCommand(
    string Code,
    Identity AccountIdentity,
    ConfirmableAction Action
) : Command { }
