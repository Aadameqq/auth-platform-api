using Core.Domain;

namespace Core.Ports;

public interface ConfirmationService
{
    Task BeginConfirmation(Account account, ConfirmableAction action);

    Task<Result<Account>> Confirm(string code, ConfirmableAction action, Guid accountId);
}
