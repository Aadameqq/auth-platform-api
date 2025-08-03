using Core.Domain;

namespace Core.Commands.Commands;

public record ConfirmActionCommand(
    Guid AccountId,
    string Code,
    ConfirmableAction Action,
    ConfirmationMethod Method,
    Guid ConfirmationId
) : Command { }
