using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Other;

public class TotpConfirmationService : ConfirmationService
{
    public Task<Result> BeginConfirmation(Account account, ConfirmableAction action)
    {
        return Task.FromResult<Result>(new RequiresNoBegin());
    }

    public Task<Result<Account>> Confirm(string code, ConfirmableAction action, Account account)
    {
        throw new NotImplementedException();
    }
}
