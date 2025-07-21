using Core.Commands.Outputs;
using Core.Domain;
using Core.Exceptions;
using Core.Ports;

namespace Core.Other;

public class SessionCreatorImpl(
    DateTimeProvider dateTimeProvider,
    AuthTokenService authTokenService
) : SessionCreator
{
    public Result<TokenPairOutput> CreateSession(Account account)
    {
        var result = account.CreateSession(dateTimeProvider.Now());

        if (result is { IsFailure: true, Exception: AccountNotActivated })
        {
            return result.Exception;
        }

        var tokenPair = authTokenService.CreateTokenPair(
            account,
            result.Value.SessionId,
            result.Value.Id
        );

        return tokenPair;
    }
}
