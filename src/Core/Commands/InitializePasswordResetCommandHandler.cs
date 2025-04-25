using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class InitializePasswordResetCommandHandler(
    UnitOfWork uow,
    PasswordResetCodesRepository codesRepository,
    PasswordResetEmailSender emailSender
) : CommandHandler<InitializePasswordResetCommand>
{
    public async Task<Result> Handle(InitializePasswordResetCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var account = await accountsRepository.FindByEmail(cmd.Email);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        if (!account.HasPassword())
        {
            return new NoPassword();
        }

        var code = await codesRepository.Create(account);

        await emailSender.Send(account, code);

        return Result.Success();
    }
}
