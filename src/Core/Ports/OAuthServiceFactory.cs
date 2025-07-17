namespace Core.Ports;

public interface OAuthServiceFactory
{
    public OAuthService? CreateInstance(string provider);
}
