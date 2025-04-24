namespace Core.Ports;

public interface UnitOfWork
{
    AccountsRepository GetAccountsRepository();
    OAuthConnectionsRepository GetOAuthConnectionsRepository();
    Task Flush();
}
