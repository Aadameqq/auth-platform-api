using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class LogOutCommandHandler(UnitOfWork uow)
{
    public async Task<Result> Execute(Guid accountId, Guid sessionId)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(accountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        account.DestroySession(sessionId);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
