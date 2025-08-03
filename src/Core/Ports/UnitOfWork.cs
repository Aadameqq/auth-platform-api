namespace Core.Ports;

public interface UnitOfWork
{
    AccountsRepository GetAccountsRepository();
    OAuthConnectionsRepository GetOAuthConnectionsRepository();
    ConfirmationsRepository GetConfirmationsRepository();

    Task<T> FailIfNull<T>(Func<Task<T?>> func)
        where T : class;

    Task Flush();
}
