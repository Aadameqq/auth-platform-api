using Core.Domain;
using Core.Dtos;
using Core.Ports;

namespace Core.Other;

public class TotpConfirmationProvider(ConfirmationService service) : ConfirmationProvider
{
    private static readonly ConfirmationMethod Method = ConfirmationMethod.Totp;

    public Task<Result<Confirmation>> Begin(ConfirmableAction action, Account account)
    {
        return service.CreateConfirmation(Method, action, account);
    }

    public Task<Result<Confirmation>> Finish(ConfirmationDto dto, string code)
    {
        throw new NotImplementedException();
    }
}
