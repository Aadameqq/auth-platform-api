using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core.Dtos;
using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.OAuth;

public class GithubOAuthService(IOptions<OAuthOptions> options, IHttpClientFactory clientFactory)
    : OAuthService
{
    public async Task<OAuthProviderTokenPair?> ExchangeCodeForAccessToken(string code)
    {
        var http = clientFactory.CreateClient();

        var baseUri = "https://github.com/login/oauth/access_token";

        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.Value.GithubClientId,
            ["client_secret"] = options.Value.GithubClientSecret,
            ["code"] = code,
        };

        var queryString = GenerateQueryString(queryParams);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUri}?{queryString}");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        Console.WriteLine(await response.Content.ReadAsStringAsync());

        var tokenResponse = await response.Content.ReadFromJsonAsync<OAuthProviderTokenPair>();

        return tokenResponse;
    }

    public async Task<OAuthUser?> GetUser(string accessToken)
    {
        var http = clientFactory.CreateClient();
        var url = "https://api.github.com/user";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<OAuthProviderTokenPair>();

        Console.WriteLine(await response.Content.ReadAsStringAsync());

        return null;
    }

    public string GenerateUrlFor(string stateToken, string redirectUri)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.Value.GithubClientId,
            ["redirect_uri"] = redirectUri,
            ["state"] = stateToken,
            ["scope"] = "user",
        };

        var queryString = GenerateQueryString(queryParams);

        return $"https://github.com/login/oauth/authorize?{queryString}";
    }

    private static string GenerateQueryString(Dictionary<string, string> queryParams)
    {
        return string.Join('&', queryParams.Select(q => $"{q.Key}={q.Value}"));
    }
}
