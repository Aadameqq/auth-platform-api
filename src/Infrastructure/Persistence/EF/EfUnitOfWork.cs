using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.EF;

public class EfUnitOfWork(IOptions<DatabaseOptions> options) : UnitOfWork, IDisposable
{
    private readonly DatabaseContext ctx = new(options);

    private readonly bool disposed = false;

    private AccountsRepository? accountsRepository;
    private OAuthConnectionsRepository? oAuthConnectionsRepository;

    public void Dispose()
    {
        if (!disposed)
        {
            ctx.Dispose();
        }
    }

    public AccountsRepository GetAccountsRepository()
    {
        if (accountsRepository is null)
        {
            accountsRepository = new EfAccountsRepository(ctx);
        }

        return accountsRepository;
    }

    public OAuthConnectionsRepository GetOAuthConnectionsRepository()
    {
        if (oAuthConnectionsRepository is null)
        {
            oAuthConnectionsRepository = new EfOAuthConnectionsRepository(ctx);
        }

        return oAuthConnectionsRepository;
    }

    public async Task Flush()
    {
        await ctx.SaveChangesAsync();
    }
}
