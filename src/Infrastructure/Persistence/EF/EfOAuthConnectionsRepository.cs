using Core.Domain;
using Core.Ports;

namespace Infrastructure.Persistence.EF;

public class EfOAuthConnectionsRepository(DatabaseContext ctx) : OAuthConnectionsRepository
{
    public Task Create(OAuthConnection connection)
    {
        throw new NotImplementedException();
    }

    public Task<OAuthConnection?> Find(string oAuthId, string provider)
    {
        throw new NotImplementedException();
    }
}
