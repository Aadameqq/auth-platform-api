using Core.Dtos;

namespace Core.Ports;

public interface OAuthStateTokenService
{
    public string Create(OAuthState state);
    public Task<OAuthState?> FetchPayloadIfValid(string stateToken);
}
