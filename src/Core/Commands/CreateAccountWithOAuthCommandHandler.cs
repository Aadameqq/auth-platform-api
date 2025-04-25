// using Core.Commands.Commands;
// using Core.Domain;
// using Core.Dtos;
// using Core.Ports;
//
// namespace Core.Commands;
//
// public class CreateAccountWithOAuthCommandHandler(
//     OAuthStateTokensService stateTokensService,
//     OAuthServiceFactory factory,
//     UnitOfWork uow
// ) : OAuthCommandHandler<CreateAccountWithOAuthCommand>(stateTokensService, factory)
// {
//     protected override async Task<Result> HandleWithAuth(
//         OAuthUser oAuthUser,
//         CreateAccountWithOAuthCommand cmd
//     )
//     {
//         var account = new Account(oAuthUser.UserName, oAuthUser.Email);
//         var connection = new OAuthConnection(account, oAuthUser.Provider, oAuthUser.OAuthId);
//
//         var accountsRepository = uow.GetAccountsRepository();
//         var connectionsRepository = uow.GetOAuthConnectionsRepository();
//
//         await accountsRepository.Create(account);
//         await connectionsRepository.Create(connection);
//
//         await uow.Flush();
//
//         return Result.Success();
//     }
// }
