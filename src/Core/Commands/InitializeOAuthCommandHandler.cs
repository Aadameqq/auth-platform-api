using Core.Dtos;
using Core.Ports;

namespace Core.Commands;

public class InitializeOAuthCommandHandler(
    OAuthStateTokensService tokensService,
    OAuthServiceFactory factory
)
{
    public async Task<string> Execute(string redirectUri, string provider)
    {
        var stateToken = await tokensService.Create(new OAuthState(provider));

        var service = factory.CreateInstance(provider);

        return service.GenerateUrlFor(stateToken, redirectUri);
    }
}
