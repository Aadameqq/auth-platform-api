using Core.Dtos;
using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.OAuth;

public class GithubOAuthService(IOptions<OAuthOptions> options, IHttpClientFactory clientFactory)
    : OAuthService
{
    public async Task<string?> ExchangeCodeForAccessToken(string code)
    {
        var http = clientFactory.CreateClient();

        var baseUri = "https://github.com/login/oauth/access_token";

        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.Value.GithubClientId,
            ["client_secret"] = options.Value.GithubClientSecret,
            ["code"] = code
        };

        var queryString = string.Join('&', queryParams.Select(q => $"{q.Key}={q.Value}"));

        var response = await http.PostAsync(new Uri($"{baseUri}?{queryString}"), null);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync();

        return response.Content // dokończ;
    }

    public Task<OAuthUser> GetUser(string accessToken)
    {
        throw new NotImplementedException();
    }

    public string GenerateUrlFor(string stateToken, string redirectUri)
    {
        throw new NotImplementedException();
    }
}
