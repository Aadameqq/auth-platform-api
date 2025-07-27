using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class CreateAccountCommandHandler(
    UnitOfWork uow,
    PasswordHasher passwordHasher,
    ActivationCodesRepository activationCodesRepository,
    ActivationCodeEmailSender codeEmailSender,
    SessionCreator sessionCreator
) : CommandHandler<CreateAccountCommand, TokenPairOutput>
{
    public async Task<Result<TokenPairOutput>> Handle(CreateAccountCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var found = await accountsRepository.FindByEmail(cmd.Email);

        if (found != null)
        {
            return new AlreadyExists<Account>();
        }

        var hashedPassword = passwordHasher.HashPassword(cmd.PlainPassword);

        var account = new Account(cmd.UserName, cmd.Email, hashedPassword);

        var code = await activationCodesRepository.Create(account);

        await codeEmailSender.Send(account, code);

        var result = sessionCreator.CreateSession(account);

        await accountsRepository.Create(account);
        await uow.Flush();

        return result;
    }
}
