using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.EF;

public class EfUnitOfWork(IOptions<DatabaseOptions> options) : UnitOfWork, IDisposable
{
    private readonly DatabaseContext ctx = new(options);

    private readonly bool disposed = false;

    private AccountsRepository? accountsRepository;
    private ConfirmationsRepository? confirmationCodesRepository;
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

    public ConfirmationsRepository GetConfirmationsRepository()
    {
        if (confirmationCodesRepository is null)
        {
            confirmationCodesRepository = new EfConfirmationsRepository(ctx);
        }

        return confirmationCodesRepository;
    }

    public async Task<T> FailIfNull<T>(Func<Task<T?>> func)
        where T : class
    {
        var result = await func();

        if (result is null)
        {
            throw new EntitySearchFailure();
        }

        return result;
    }

    public async Task Flush()
    {
        await ctx.SaveChangesAsync();
    }
}
