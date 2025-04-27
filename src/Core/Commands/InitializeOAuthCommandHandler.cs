using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class InitializeOAuthCommandHandler(
    OAuthStateTokensService tokensService,
    OAuthServiceFactory factory
) : CommandHandler<InitializeOAuthCommand, OAuthUrlOutput>
{
    public async Task<Result<OAuthUrlOutput>> Handle(
        InitializeOAuthCommand cmd,
        CancellationToken _
    )
    {
        var state = new OAuthState(cmd.Provider);
        var stateToken = await tokensService.Create(state);

        var service = factory.CreateInstance(cmd.Provider);

        if (service is null)
        {
            return new InvalidOAuthProvider();
        }

        var url = service.GenerateUrlFor(stateToken, cmd.RedirectUri); // TODO: make url/uri naming consistent

        return new OAuthUrlOutput(url, state.Id);
    }
}
