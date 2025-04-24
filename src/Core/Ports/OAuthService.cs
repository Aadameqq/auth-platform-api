using Core.Dtos;

namespace Core.Ports;

public interface OAuthService
{
    public Task<string> ExchangeCodeForAccessToken(string code);
    public Task<OAuthUser> GetUser(string accessToken);
    public string GenerateUrlFor(string stateToken, string redirectUri);
}
