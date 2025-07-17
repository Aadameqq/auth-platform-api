using Core.Commands.Outputs;

namespace Core.Commands.Commands;

public record LogInCommand(string Email, string Password) : Command<TokenPairOutput>;
