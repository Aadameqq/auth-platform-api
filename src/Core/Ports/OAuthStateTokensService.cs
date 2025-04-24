using Core.Dtos;

namespace Core.Ports;

public interface OAuthStateTokensService
{
    public Task<string> Create(OAuthState state);
    public Task<OAuthState?> FetchPayloadIfValid(string stateToken);
}
