using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class LogInWithOAuthCommandHandler(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory,
    UnitOfWork uow,
    SessionCreator sessionCreator
) : OAuthCommandHandler<LogInWithOAuthCommand, TokenPairOutput>(stateTokensService, factory)
{
    protected override async Task<Result<TokenPairOutput>> HandleWithAuth(
        OAuthUser oAuthUser,
        LogInWithOAuthCommand cmd
    )
    {
        var accountsRepository = uow.GetAccountsRepository();
        var connectionsRepository = uow.GetOAuthConnectionsRepository();

        var connection = await connectionsRepository.Find(oAuthUser.OAuthId, oAuthUser.Provider);

        if (connection is null)
        {
            return new NoSuch<OAuthConnection>();
        }

        var account = await accountsRepository.FindById(connection.AccountId);

        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var result = sessionCreator.CreateSession(account);

        if (result is { IsFailure: true, Exception: AccountNotActivated })
        {
            return result.Exception;
        }

        await accountsRepository.Update(account);
        await uow.Flush();

        return result.Value;
    }
}
