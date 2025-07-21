using System.Security.Claims;
using Core.Dtos;
using Core.Ports;
using Infrastructure.Options;
using Infrastructure.Other;
using Microsoft.Extensions.Options;

namespace Infrastructure.OAuth;

public class SystemOAuthStateTokenService(IOptions<OAuthOptions> options) : OAuthStateTokenService
{
    private const string StateIdClaimType = "stateId";
    private const string ProviderClaimType = "provider";

    private readonly JwtService jwtService = new(
        options.Value.StateTokenSecret,
        true,
        options.Value.StateTokenLifetimeInMinutes
    );

    public string Create(OAuthState state)
    {
        var claims = new List<Claim>
        {
            new(StateIdClaimType, state.Id.ToString()),
            new(ProviderClaimType, state.Provider),
        };

        return jwtService.SignToken(claims);
    }

    public async Task<OAuthState?> FetchPayloadIfValid(string stateToken)
    {
        var principal = await jwtService.FetchPayloadIfValid(stateToken);

        if (principal is null)
        {
            return null;
        }

        var stateId = Guid.Parse(JwtService.GetClaim(principal, StateIdClaimType));
        var provider = JwtService.GetClaim(principal, ProviderClaimType);

        return new OAuthState(stateId, provider);
    }
}
