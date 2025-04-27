using Core.Dtos;
using Core.Ports;

namespace Infrastructure.OAuth;

public class SystemOAuthStateTokensService : OAuthStateTokensService
{
    public Task<string> Create(OAuthState state)
    {
        throw new NotImplementedException();
    }

    public Task<OAuthState?> FetchPayloadIfValid(string stateToken)
    {
        throw new NotImplementedException();
    }
}
