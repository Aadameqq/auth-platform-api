using Core.Domain;

namespace Core.Commands.Commands;

public record ResetPasswordCommand(string Code, Guid AccountId, string NewPassword)
    : RequireConfirmationCommand(Code, AccountId, ConfirmableAction.PasswordReset);
