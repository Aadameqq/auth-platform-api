using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class RefreshTokensCommandHandler(
    UnitOfWork uow,
    DateTimeProvider dateTimeProvider,
    AuthTokenService authTokenService
) : CommandHandler<RefreshTokensCommand, TokenPairOutput>
{
    public async Task<Result<TokenPairOutput>> Handle(RefreshTokensCommand cmd, CancellationToken _)
    {
        var accountsRepository = uow.GetAccountsRepository();
        var payload = await authTokenService.FetchRefreshTokenPayloadIfValid(cmd.Token);

        if (payload is null)
        {
            return new InvalidToken();
        }

        var account = await accountsRepository.FindById(payload.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = account.RefreshSession(payload.ToRefreshToken(), dateTimeProvider.Now());

        if (result is { IsFailure: true, Exception: NoSuch<AuthSession> or InvalidToken })
        {
            await accountsRepository.Update(account);
            await uow.Flush();
            return result.Exception;
        }

        var pair = authTokenService.CreateTokenPair(account, result.Value.SessionId, result.Value.Id);

        await accountsRepository.Update(account);
        await uow.Flush();
        return pair;
    }
}
