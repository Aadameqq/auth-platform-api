using Core.Domain;
using Core.Ports;

namespace Core.Other;

public class ConfirmationCodeIdentity(UnitOfWork uow, string code) : Identity
{
    public async Task<Account?> Get()
    {
        var confirmationCodeRepository = uow.GetConfirmationCodesRepository();
        var accountsRepository = uow.GetAccountsRepository();
        var foundCode = await confirmationCodeRepository.FindByCode(code);

        if (foundCode is null)
        {
            return null;
        }

        return await accountsRepository.FindById(foundCode.OwnerId);
    }

    public Task<Account> GetOrFail()
    {
        return uow.FailIfNull(Get);
    }
}
