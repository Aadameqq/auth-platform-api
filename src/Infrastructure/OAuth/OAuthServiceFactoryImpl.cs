using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.OAuth;

public class OAuthServiceFactoryImpl(IOptions<OAuthOptions> options, IHttpClientFactory factory)
    : OAuthServiceFactory
{
    public OAuthService? CreateInstance(string provider)
    {
        return provider switch
        {
            "github" => new GithubOAuthService(options, factory),
            _ => null,
        };
    }
}
