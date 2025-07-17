using Core.Commands.Outputs;

namespace Core.Commands.Commands;

public record RefreshTokensCommand(string Token) : Command<TokenPairOutput>;
