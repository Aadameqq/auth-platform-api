using Core.Domain;
using Core.Ports;

namespace Core.Other;

public class EmailIdentity(UnitOfWork uow, string email) : Identity
{
    public Task<Account> GetOrFail()
    {
        var accountsRepository = uow.GetAccountsRepository();
        return accountsRepository.FindByEmailOrFail(email);
    }

    public Task<Account?> Get()
    {
        var accountsRepository = uow.GetAccountsRepository();
        return accountsRepository.FindByEmail(email);
    }
}
