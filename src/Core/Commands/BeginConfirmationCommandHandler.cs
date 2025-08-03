using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Other;
using Core.Ports;

namespace Core.Commands;

public class BeginConfirmationCommandHandler(
    UnitOfWork uow,
    ConfirmationProviderFactory providerFactory
) : CommandHandler<BeginConfirmationCommand, Confirmation>
{
    public async Task<Result<Confirmation>> Handle(
        BeginConfirmationCommand cmd,
        CancellationToken _
    )
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.AccountId));

        var provider = providerFactory.CreateInstance(cmd.Method);

        var result = await provider.Begin(cmd.Action, account);

        if (result is { IsFailure: true, Exception: TooManyAttempts })
        {
            return result.Exception;
        }

        return result.Value;
    }
}
