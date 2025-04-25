using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class AddOAuthConnectionCommandHandler(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory,
    UnitOfWork uow
) : OAuthCommandHandler<AddOAuthConnectionCommand>(stateTokensService, factory)
{
    protected override async Task<Result> HandleWithAuth(
        OAuthUser oAuthUser,
        AddOAuthConnectionCommand cmd
    )
    {
        var accountsRepository = uow.GetAccountsRepository();
        var connectionsRepository = uow.GetOAuthConnectionsRepository();

        var account = await accountsRepository.FindById(cmd.AccountId);
        if (account is null)
        {
            return new NoSuch<Account>();
        }

        var existingConnection = await connectionsRepository.Find(
            oAuthUser.OAuthId,
            oAuthUser.Provider // TODO: it should be searched by AccountId instead of OAuthId
        );

        if (existingConnection is not null)
        {
            return new AlreadyExists<OAuthConnection>();
        }

        var connection = new OAuthConnection(account, oAuthUser.Provider, oAuthUser.OAuthId);

        await connectionsRepository.Create(connection);

        await uow.Flush();

        return Result.Success();
    }
}
