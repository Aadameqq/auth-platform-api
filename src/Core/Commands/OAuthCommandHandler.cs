using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public abstract class OAuthCommandHandler(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory
)
{
    protected async Task<Result<OAuthUser>> Authorize(OAuthCommand cmd)
    {
        var state = await stateTokensService.FetchPayloadIfValid(cmd.StateToken);

        if (state is null || state.Id != cmd.StateId)
        {
            return new InvalidState();
        }

        var service = factory.CreateInstance(state.Provider);
        var providerToken = await service.ExchangeCodeForAccessToken(cmd.Code); // TODO: it can fail

        return await service.GetUser(providerToken);
    }
}

public abstract class OAuthCommandHandler<TCommand, TOutput>(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory
) : OAuthCommandHandler(stateTokensService, factory), CommandHandler<TCommand, TOutput>
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
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory
) : OAuthCommandHandler(stateTokensService, factory), CommandHandler<TCommand>
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
