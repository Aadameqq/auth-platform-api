using Core.Commands.Outputs;

namespace Core.Commands.Commands;

public record InitializeOAuthCommand(string RedirectUri, string Provider) : Command<OAuthUrlOutput>;
