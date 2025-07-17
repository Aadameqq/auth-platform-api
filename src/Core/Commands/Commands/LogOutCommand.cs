namespace Core.Commands.Commands;

public record LogOutCommand(Guid AccountId, Guid SessionId) : Command;
