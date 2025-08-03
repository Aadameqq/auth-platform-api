using Core.Domain;
using Core.Other;

namespace Core.Commands.Commands;

public record ConfirmActionCommand(
    Guid AccountId,
    string Code,
    ConfirmableAction Action,
    ConfirmationMethod Method
) : Command { }
