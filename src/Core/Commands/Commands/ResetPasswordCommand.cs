namespace Core.Commands.Commands;

public record ResetPasswordCommand(string Code, string NewPassword) : Command;
