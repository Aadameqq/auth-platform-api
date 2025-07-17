using Core.Commands.Outputs;

namespace Core.Commands.Commands;

public record InitializeOAuthCommand(string Provider) : Command<OAuthUrlOutput>;
