using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Core.Dtos;
using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.OAuth;

public class GithubOAuthService(IOptions<OAuthOptions> options, IHttpClientFactory clientFactory)
    : OAuthService
{
    public const string ProviderName = "github";

    public async Task<OAuthProviderTokenPair?> ExchangeCodeForAccessToken(string code)
    {
        var http = clientFactory.CreateClient();

        var baseUri = "https://github.com/login/oauth/access_token";

        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.Value.GithubClientId,
            ["client_secret"] = options.Value.GithubSecret,
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

        var tokenResponse = await response.Content.ReadFromJsonAsync<CodeExchangeResponse>();

        return tokenResponse is null ? null : new OAuthProviderTokenPair(tokenResponse.AccessToken);
    }

    public async Task<OAuthUser?> GetUser(string accessToken)
    {
        var http = clientFactory.CreateClient();
        var url = "https://api.github.com/user";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        ApplyUserEndpointHeaders(request, accessToken);

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        var emailUrl = "https://api.github.com/user/emails";
        using var emailRequest = new HttpRequestMessage(HttpMethod.Get, emailUrl);

        ApplyUserEndpointHeaders(emailRequest, accessToken);

        response = await http.SendAsync(emailRequest);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var emailsResponse = await response.Content.ReadFromJsonAsync<EmailResponse[]>();

        if (emailsResponse is null)
        {
            return null;
        }

        var email = GetPrimaryEmail(emailsResponse);

        if (email is null)
        {
            return null;
        }

        return userResponse is null
            ? null
            : new OAuthUser(
                userResponse.Id.ToString(),
                userResponse.Login,
                email.Email,
                ProviderName,
                email.IsVerified
            );
    }

    public string GenerateUrlFor(string stateToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.Value.GithubClientId,
            ["redirect_uri"] = options.Value.OAuthRedirectUrl,
            ["state"] = stateToken,
            ["scope"] = "read:user user:email",
        };

        var queryString = GenerateQueryString(queryParams);

        return $"https://github.com/login/oauth/authorize?{queryString}";
    }

    private static string GenerateQueryString(Dictionary<string, string> queryParams)
    {
        return string.Join(
            '&',
            queryParams.Select(q => $"{q.Key}={Uri.EscapeDataString(q.Value)}")
        );
    }

    private void ApplyUserEndpointHeaders(HttpRequestMessage request, string accessToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Add(
            new ProductInfoHeaderValue(options.Value.UserAgentName, options.Value.UserAgentVersion)
        );
    }

    private EmailResponse? GetPrimaryEmail(EmailResponse[] emails)
    {
        foreach (var email in emails)
        {
            if (email is { IsPrimary: true })
            {
                return email;
            }
        }

        return null;
    }

    private class CodeExchangeResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }
    }

    private class EmailResponse
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("primary")]
        public required bool IsPrimary { get; init; }

        [JsonPropertyName("verified")]
        public required bool IsVerified { get; init; }
    }

    private class UserResponse
    {
        [JsonPropertyName("id")]
        public required int Id { get; init; }

        [JsonPropertyName("login")]
        public required string Login { get; init; }

        [JsonPropertyName("email")]
        public required string Email { get; init; }
    }
}
