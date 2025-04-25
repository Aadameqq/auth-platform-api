using Core.Commands.Commands;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class InitializePasswordResetCommandHandler(
    AccountsRepository accountsRepository,
    PasswordResetCodesRepository codesRepository,
    PasswordResetEmailSender emailSender
) : CommandHandler<InitializePasswordResetCommand>
{
    public async Task<Result> Handle(InitializePasswordResetCommand cmd)
    {
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

        _ = emailSender.Send(account, code);

        return Result.Success();
    }
}
