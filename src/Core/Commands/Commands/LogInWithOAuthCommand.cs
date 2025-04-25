namespace Core.Commands.Commands;

public record LogInWithOAuthCommand(string StateToken, Guid StateId, string Code)
    : OAuthCommand(StateToken, StateId, Code);
