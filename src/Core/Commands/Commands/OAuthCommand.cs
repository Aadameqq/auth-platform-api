namespace Core.Commands.Commands;

public abstract record OAuthCommand<TOutput>(string StateToken, Guid StateId, string Code)
    : Command<TOutput> { }

public abstract record OAuthCommand(string StateToken, Guid StateId, string Code) : Command { }
