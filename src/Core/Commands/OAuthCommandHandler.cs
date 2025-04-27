using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public abstract class OAuthCommandHandler(
    OAuthStateTokenService stateTokenService,
    OAuthServiceFactory factory
)
{
    protected async Task<Result<OAuthUser>> Authorize(OAuthCommand cmd)
    {
        var state = await stateTokenService.FetchPayloadIfValid(cmd.StateToken);

        if (state is null || state.Id != cmd.StateId)
        {
            return new InvalidState();
        }

        var service = factory.CreateInstance(state.Provider);

        if (service is null)
        {
            return new InvalidOAuthProvider();
        }

        var providerToken = await service.ExchangeCodeForAccessToken(cmd.Code);

        if (providerToken is null)
        {
            return new InvalidOAuthCode();
        }

        var user = await service.GetUser(providerToken.AccessToken);

        if (user is null)
        {
            return new OAuthProviderConnectionFailure();
        }

        return user;
    }
}

public abstract class OAuthCommandHandler<TCommand, TOutput>(
    OAuthStateTokenService stateTokenService,
    OAuthServiceFactory factory
) : OAuthCommandHandler(stateTokenService, factory), CommandHandler<TCommand, TOutput>
    where TCommand : OAuthCommand<TOutput>
    where TOutput : class
{
    public async Task<Result<TOutput>> Handle(TCommand command, CancellationToken _)
    {
        var result = await Authorize(command);
        if (result.IsFailure)
        {
            return result.Exception;
        }

        return await HandleWithAuth(result.Value, command);
    }

    protected abstract Task<Result<TOutput>> HandleWithAuth(OAuthUser user, TCommand command);
}

public abstract class OAuthCommandHandler<TCommand>(
    OAuthStateTokenService stateTokenService,
    OAuthServiceFactory factory
) : OAuthCommandHandler(stateTokenService, factory), CommandHandler<TCommand>
    where TCommand : OAuthCommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken _)
    {
        var result = await Authorize(command);
        if (result.IsFailure)
        {
            return result.Exception;
        }

        return await HandleWithAuth(result.Value, command);
    }

    protected abstract Task<Result> HandleWithAuth(OAuthUser user, TCommand command);
}
