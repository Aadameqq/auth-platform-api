using Core.Domain;
using Core.Ports;

namespace Core.Commands.Commands;

public record ActivateAccountCommand(Identity AccountIdentity, string Code)
    : RequireConfirmationCommand(Code, AccountIdentity, ConfirmableAction.AccountActivation) { }
