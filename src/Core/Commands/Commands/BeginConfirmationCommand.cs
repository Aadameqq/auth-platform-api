using Core.Domain;
using Core.Other;

namespace Core.Commands.Commands;

public record BeginConfirmationCommand(
    Guid AccountId,
    ConfirmableAction Action,
    ConfirmationMethod Method
) : Command;
