using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Other;

public class ConfirmationServiceImpl(
    ConfirmationCodeEmailSender emailSender,
    DateTimeProvider dateTimeProvider,
    UnitOfWork uow
) : ConfirmationService
{
    public async Task BeginConfirmation(Account account, ConfirmableAction action)
    {
        var repository = uow.GetConfirmationCodesRepository();
        var code = repository.GenerateCode();
        var confirmationCode = new ConfirmationCode(account, code, dateTimeProvider.Now(), action);

        await repository.Create(confirmationCode);
        await uow.Flush();

        await emailSender.Send(account, confirmationCode);
    }

    public async Task<Result<Account>> Confirm(
        string code,
        ConfirmableAction action,
        Guid accountId
    )
    {
        var codesRepository = uow.GetConfirmationCodesRepository();
        var confirmationCode = await codesRepository.FindByCode(code, action);

        if (confirmationCode is null || confirmationCode.HasExpired(dateTimeProvider.Now()))
        {
            return new Expired();
        }

        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindById(accountId);

        if (account is null || !confirmationCode.IsOwner(account))
        {
            return new NoSuch<Account>();
        }

        return account;
    }
}
