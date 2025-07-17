namespace Core.Commands.Commands;

public record AddOAuthConnectionCommand(
    string StateToken,
    Guid StateId,
    string Code,
    Guid AccountId
) : OAuthCommand(StateToken, StateId, Code);
