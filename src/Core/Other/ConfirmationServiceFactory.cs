using Core.Ports;

namespace Core.Other;

public class ConfirmationServiceFactory(
    ConfirmationCodeEmailSender emailSender,
    DateTimeProvider dateTimeProvider,
    UnitOfWork uow
)
{
    public ConfirmationService CreateInstance(ConfirmationMethod method)
    {
        return method switch
        {
            ConfirmationMethod.Email => new EmailConfirmationService(
                emailSender,
                dateTimeProvider,
                uow
            ),
            ConfirmationMethod.Totp => new TotpConfirmationService(),
            ConfirmationMethod.Preference => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
