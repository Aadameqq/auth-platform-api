using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class LogOutCommandHandler(UnitOfWork uow) : CommandHandler<LogOutCommand>
{
    public async Task<Result> Handle(LogOutCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(cmd.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        account.DestroySession(cmd.SessionId);

        await accountsRepository.Update(account);
        await uow.Flush();

        return Result.Success();
    }
}
