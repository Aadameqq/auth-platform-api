using Core.Domain;

namespace Core.Commands.Commands;

public record AssignRoleCommand(Guid IssuerId, Guid AccountId, Role Role) : Command;
