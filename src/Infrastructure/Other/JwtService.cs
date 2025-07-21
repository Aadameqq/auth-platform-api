using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Other;

public class JwtService(string stringKey, bool hasLifetime = true, int lifetimeInMinutes = 0)
{
    private readonly string issuer = "localhost";
    private readonly SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(stringKey));

    public static string GetClaim(ClaimsPrincipal principal, string claimType)
    {
        var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        if (claim is null)
        {
            throw new InvalidOperationException("No claim found for claim type");
        }

        return claim;
    }

    public string SignToken(List<Claim> claims)
    {
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            claims: claims,
            issuer: issuer,
            audience: "*",
            notBefore: hasLifetime ? now : null,
            expires: hasLifetime ? now.Add(TimeSpan.FromMinutes(lifetimeInMinutes)) : null,
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<ClaimsPrincipal?> FetchPayloadIfValid(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = hasLifetime,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = false,
            IssuerSigningKey = signingKey,
        };

        try
        {
            var principal = await Task.Run(
                () =>
                    new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _)
            );

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
