using Core.Domain;
using Core.Dtos;
using Core.Ports;

namespace Core.UseCases;

public class CreateAccountWithOAuthUseCase(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory,
    UnitOfWork uow
) : OAuthAuthorizationUseCase<object, object>(stateTokensService, factory)
{
    protected override async Task<Result<object>> ExecuteWithAuth(OAuthUser oAuthUser, object _)
    {
        var account = new Account(oAuthUser.UserName, oAuthUser.Email);
        var connection = new OAuthConnection(account, oAuthUser.Provider, oAuthUser.OAuthId);

        var accountsRepository = uow.GetAccountsRepository();
        var connectionsRepository = uow.GetOAuthConnectionsRepository();

        await accountsRepository.Create(account);
        await connectionsRepository.Create(connection);

        await uow.Flush();

        return Result.Success();
    }
}
