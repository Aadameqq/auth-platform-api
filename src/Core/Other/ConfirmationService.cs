using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Other;

public class ConfirmationService(UnitOfWork uow, DateTimeProvider dateTimeProvider)
{
    public async Task<Result<Confirmation>> CreateConfirmation(
        ConfirmationMethod method,
        ConfirmableAction action,
        Account account,
        string? code = null
    )
    {
        var repository = uow.GetConfirmationsRepository();
        var found = await repository.FindByAccount(account, action);

        if (found is not null)
        {
            if (found.IsCooldown(dateTimeProvider.Now()))
            {
                return new TooManyAttempts();
            }

            if (found.DoesMethodEqual(method))
            {
                _ = repository.Delete(found);
            }
        }

        var confirmation = new Confirmation(account, dateTimeProvider.Now(), action, method, code);

        await repository.Create(confirmation);
        await uow.Flush();

        return confirmation;
    }

    public async Task<Result<Confirmation>> GetConfirmation(ConfirmationDto dto)
    {
        var repository = uow.GetConfirmationsRepository();

        var confirmation = await repository.FindById(dto.Id);

        if (confirmation is null)
        {
            return new NoSuch();
        }

        var result = confirmation.Check(dateTimeProvider.Now(), dto.Method, dto.Action);

        if (result is { IsFailure: true, Exception: ConfirmationMismatch })
        {
            return result.Exception;
        }

        await repository.Delete(confirmation);
        await uow.Flush();

        if (result is { IsFailure: true, Exception: Expired })
        {
            return result.Exception;
        }

        return confirmation;
    }

    public string GenerateCode()
    {
        return uow.GetConfirmationsRepository().GenerateCode();
    }
}
