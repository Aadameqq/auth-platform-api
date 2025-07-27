using Core.Domain;

namespace Core.Commands.Commands;

public record RequireConfirmationCommand<TOutput>(
    string Code,
    Guid AccountId,
    ConfirmableAction Action
) : Command<TOutput> { }

public record RequireConfirmationCommand(string Code, Guid AccountId, ConfirmableAction Action)
    : Command { }
