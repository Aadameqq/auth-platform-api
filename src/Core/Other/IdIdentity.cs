using Core.Domain;
using Core.Ports;

namespace Core.Other;

public class IdIdentity(UnitOfWork uow, Guid id) : Identity
{
    public Task<Account?> Get()
    {
        var accountsRepository = uow.GetAccountsRepository();
        return accountsRepository.FindById(id);
    }

    public Task<Account> GetOrFail()
    {
        var accountsRepository = uow.GetAccountsRepository();
        return accountsRepository.FindByIdOrFail(id);
    }
}
