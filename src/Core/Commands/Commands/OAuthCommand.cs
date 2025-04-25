namespace Core.Commands.Commands;

public abstract record OAuthCommand(string StateToken, Guid StateId, string Code) : Command
{
}
