using Core.Domain;
using Core.Ports;

namespace Core.Other;

public class ConfirmationProviderFactoryImpl(
    ConfirmationCodeEmailSender emailSender,
    ConfirmationService confirmationService
) : ConfirmationProviderFactory
{
    public ConfirmationProvider CreateInstance(ConfirmationMethod method)
    {
        return method switch
        {
            ConfirmationMethod.Email => new EmailConfirmationProviderImpl(
                emailSender,
                confirmationService
            ),
            ConfirmationMethod.Totp => new TotpConfirmationProvider(confirmationService),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
