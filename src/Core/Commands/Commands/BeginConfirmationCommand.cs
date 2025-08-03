using Core.Domain;

namespace Core.Commands.Commands;

public record BeginConfirmationCommand(
    Guid AccountId,
    ConfirmableAction Action,
    ConfirmationMethod Method
) : Command<Confirmation>;
