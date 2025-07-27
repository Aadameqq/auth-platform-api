namespace Core.Ports;

public interface UnitOfWork
{
    AccountsRepository GetAccountsRepository();
    OAuthConnectionsRepository GetOAuthConnectionsRepository();
    ConfirmationCodesRepository GetConfirmationCodesRepository();
    Task Flush();
}
