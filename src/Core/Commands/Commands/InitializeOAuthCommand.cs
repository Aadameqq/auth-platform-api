namespace Core.Commands.Commands;

public record InitializeOAuthCommand(string RedirectUri, string Provider) : Command;
