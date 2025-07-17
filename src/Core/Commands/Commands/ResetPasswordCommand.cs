namespace Core.Commands.Commands;

public record ResetPasswordCommand(string ResetCode, string NewPassword) : Command;
