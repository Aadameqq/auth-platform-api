namespace Core.Ports;

public interface RevokedTokensRepository
{
    Task Revoke(string token, TimeSpan lifeTimeLeft);
    Task<bool> HasBeenRevoked(string token);
}
