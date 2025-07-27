using Core.Domain;

namespace Core.Ports;

public interface Identity
{
    Task<Account?> Get();
    Task<Account> GetOrFail();
}
