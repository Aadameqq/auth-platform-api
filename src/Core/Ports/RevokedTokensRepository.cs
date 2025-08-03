namespace Core.Ports;

public interface RevokedTokensRepository
{
    Task Revoke(string token);
    Task<bool> HasBeenRevoked(string token);
}
