using Core.Commands.Outputs;

namespace Core.Commands.Commands;

public record LogInWithOAuthCommand(string StateToken, Guid StateId, string Code)
    : OAuthCommand<TokenPairOutput>(StateToken, StateId, Code);
