using Core.Domain;
using Core.Ports;

namespace Core.Commands.Commands;

public record ResetPasswordCommand(string Code, Identity AccountIdentity, string NewPassword)
    : RequireConfirmationCommand(Code, AccountIdentity, ConfirmableAction.PasswordReset);
