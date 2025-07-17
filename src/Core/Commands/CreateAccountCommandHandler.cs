using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class CreateAccountCommandHandler(
    UnitOfWork uow,
    PasswordHasher passwordHasher,
    ActivationCodesRepository activationCodesRepository,
    ActivationCodeEmailSender codeEmailSender
) : CommandHandler<CreateAccountCommand>
{
    public async Task<Result> Handle(CreateAccountCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var found = await accountsRepository.FindByEmail(cmd.Email);

        if (found != null)
        {
            return new AlreadyExists<Account>();
        }

        var hashedPassword = passwordHasher.HashPassword(cmd.PlainPassword);

        var account = new Account(cmd.UserName, cmd.Email, hashedPassword);

        await accountsRepository.Create(account);

        var code = await activationCodesRepository.Create(account);

        await codeEmailSender.Send(account, code);

        await uow.Flush();

        return Result.Success();
    }
}
