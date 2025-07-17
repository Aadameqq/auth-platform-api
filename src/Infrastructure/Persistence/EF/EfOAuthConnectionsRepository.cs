using Core.Domain;
using Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EF;

public class EfOAuthConnectionsRepository(DatabaseContext ctx) : OAuthConnectionsRepository
{
    public async Task Create(OAuthConnection connection)
    {
        await ctx.OAuthConnections.AddAsync(connection);
    }

    public Task<OAuthConnection?> Find(string oAuthId, string provider)
    {
        return ctx.OAuthConnections.FirstOrDefaultAsync(c =>
            c.OAuthId == oAuthId && c.Provider == provider
        );
    }

    public Task<OAuthConnection?> Find(Guid accountId, string provider)
    {
        return ctx.OAuthConnections.FirstOrDefaultAsync(c =>
            c.AccountId == accountId && c.Provider == provider
        );
    }
}
