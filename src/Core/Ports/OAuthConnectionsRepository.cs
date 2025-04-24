using Core.Domain;

namespace Core.Ports;

public interface OAuthConnectionsRepository
{
    Task Create(OAuthConnection connection);
    Task<OAuthConnection?> Find(string oAuthId, string provider);
}
