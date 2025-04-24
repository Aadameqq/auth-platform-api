using Core.Dtos;
using Core.Ports;

namespace Core.UseCases;

public class InitializeOAuthUseCase(
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
