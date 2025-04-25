using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;
using Core.Ports;

namespace Core.Commands;

public class InitializeOAuthCommandHandler(
    OAuthStateTokensService tokensService,
    OAuthServiceFactory factory
) : CommandHandler<InitializeOAuthCommand, OAuthUriOutput>
{
    public async Task<Result<OAuthUriOutput>> Handle(InitializeOAuthCommand cmd)
    {
        var stateToken = await tokensService.Create(new OAuthState(cmd.Provider));

        var service = factory.CreateInstance(cmd.Provider);

        var uri = service.GenerateUrlFor(stateToken, cmd.RedirectUri);

        return new OAuthUriOutput(uri);
    }
}
