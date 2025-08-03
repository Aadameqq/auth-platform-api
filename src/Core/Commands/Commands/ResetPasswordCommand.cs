namespace Core.Commands.Commands;

public record ResetPasswordCommand(Guid ConfirmationId, string Code, string NewPassword) : Command;
