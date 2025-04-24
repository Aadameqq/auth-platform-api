using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.UseCases;

public abstract class OAuthAuthorizationUseCase<TInput, TOutput>(
    OAuthStateTokensService stateTokensService,
    OAuthServiceFactory factory
)
{
    public async Task<Result<TOutput>> Execute(
        string stateToken,
        Guid stateId,
        string code,
        TInput input
    )
    {
        var state = await stateTokensService.FetchPayloadIfValid(stateToken);

        if (state is null || state.Id != stateId)
        {
            return new InvalidState();
        }

        var service = factory.CreateInstance(state.Provider);
        var providerToken = await service.ExchangeCodeForAccessToken(code); // TODO: it can fail

        var user = await service.GetUser(providerToken);

        return await ExecuteWithAuth(user, input);
    }

    protected abstract Task<Result<TOutput>> ExecuteWithAuth(OAuthUser oAuthUser, TInput input);
}
