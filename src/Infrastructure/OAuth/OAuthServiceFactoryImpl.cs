using Core.Ports;

namespace Infrastructure.OAuth;

public class OAuthServiceFactoryImpl : OAuthServiceFactory
{
    public OAuthService? CreateInstance(string provider)
    {
        return provider switch
        {
            "github" => new GithubOAuthService(),
            _ => null,
        };
    }
}
