using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Other;
using Core.Ports;

namespace Core.Commands;

public class ConfirmActionCommandHandler(
    UnitOfWork uow,
    ConfirmationProviderFactory providerFactory
) : CommandHandler<ConfirmActionCommand>
{
    public async Task<Result> Handle(ConfirmActionCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await uow.FailIfNull(() => accountsRepository.FindById(cmd.AccountId));

        var provider = providerFactory.CreateInstance(cmd.Method);

        var result = await provider.Finish(
            new ConfirmationDto(cmd.ConfirmationId, cmd.Method, cmd.Action),
            cmd.Code
        );

        if (
            result is
            {
                IsFailure: true,
                Exception: NoSuch or Expired or ConfirmationMismatch or InvalidConfirmationCode
            }
        )
        {
            return result.Exception;
        }

        if (!result.Value.IsOwner(account))
        {
            return new NoSuch();
        }

        return Result.Success();
    }
}
