namespace Core.Commands.Commands;

public record UnassignRoleCommand(Guid IssuerId, Guid AccountId) : Command;
