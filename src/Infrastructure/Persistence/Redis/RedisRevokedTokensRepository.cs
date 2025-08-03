using Core.Ports;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Redis;

public class RedisRevokedTokensRepository(IConnectionMultiplexer redis) : RevokedTokensRepository
{
    public async Task Revoke(string token)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync(token, true);
    }

    public async Task<bool> HasBeenRevoked(string token)
    {
        var db = redis.GetDatabase();
        return await db.StringGetAsync(token) == true;
    }
}
