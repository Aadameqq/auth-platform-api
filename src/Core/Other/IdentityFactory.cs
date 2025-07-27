using Core.Ports;

namespace Core.Other;

public class IdentityFactory(UnitOfWork uow)
{
    public Identity CreateEmailIdentity(string email)
    {
        return new EmailIdentity(uow, email);
    }

    public Identity CreateIdIdentity(Guid id)
    {
        return new IdIdentity(uow, id);
    }

    public Identity CreateCodeIdentity(string code)
    {
        return new ConfirmationCodeIdentity(uow, code);
    }
}
