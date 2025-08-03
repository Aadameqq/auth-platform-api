using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Other;

public class EmailConfirmationProviderImpl(
    ConfirmationCodeEmailSender emailSender,
    ConfirmationService service
) : EmailConfirmationProvider
{
    private static readonly ConfirmationMethod Method = ConfirmationMethod.Email;

    public async Task<Result<Confirmation>> Finish(ConfirmationDto dto, string code)
    {
        var result = await service.GetConfirmation(dto);

        if (result is { IsFailure: true, Exception: NoSuch or Expired or ConfirmationMismatch })
        {
            return result.Exception;
        }

        var confirmation = result.Value;

        if (!confirmation.IsCodeCorrect(code))
        {
            return new InvalidConfirmationCode();
        }

        return confirmation;
    }

    public async Task<Result<Confirmation>> Begin(ConfirmableAction action, Account account)
    {
        var confirmationResult = await service.CreateConfirmation(
            Method,
            action,
            account,
            service.GenerateCode()
        );

        if (confirmationResult is { IsFailure: true, Exception: TooManyAttempts })
        {
            return confirmationResult.Exception;
        }

        await emailSender.Send(account, confirmationResult.Value);

        return confirmationResult.Value;
    }
}
