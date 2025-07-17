using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;

namespace Core.Ports;

public interface AuthTokenService
{
    TokenPairOutput CreateTokenPair(Account account, Guid sessionId, Guid tokenId);
    Task<AccessTokenPayload?> FetchPayloadIfValid(string accessToken);
    Task<RefreshTokenPayload?> FetchRefreshTokenPayloadIfValid(string refreshToken);
}
