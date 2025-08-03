namespace Core.Commands.Commands;

public record InitializePasswordResetCommand(string Email) : Command { }
