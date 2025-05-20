namespace Core.Domain;

public class OAuthConnection
{
    public readonly Guid AccountId;
    public readonly Guid Id = Guid.NewGuid();
    public readonly string OAuthId;
    public readonly string Provider;

    public OAuthConnection(Account account, string provider, string oAuthId)
    {
        Provider = provider;
        AccountId = account.Id;
        OAuthId = oAuthId;
    }

#pragma warning disable CS8618
    private OAuthConnection() { }
#pragma warning restore CS8618
}
