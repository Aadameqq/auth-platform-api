using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.UseCases;

public class RefreshTokensUseCase(
    AccountsRepository accountsRepository,
    DateTimeProvider dateTimeProvider,
    TokenService tokenService
)
{
    public async Task<Result<TokenPairOutput>> Execute(string token)
    {
        var payload = await tokenService.FetchRefreshTokenPayloadIfValid(token);

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
            return result.Exception;
        }

        var pair = tokenService.CreateTokenPair(account, payload.SessionId);

        await accountsRepository.UpdateAndFlush(account);

        return pair;
    }
}
