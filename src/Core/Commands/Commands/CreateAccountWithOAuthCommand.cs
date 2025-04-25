namespace Core.Commands.Commands;

public record CreateAccountWithOAuthCommand(string StateToken, Guid StateId, string Code)
    : OAuthCommand(StateToken, StateId, Code);
