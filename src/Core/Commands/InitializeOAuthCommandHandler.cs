using Core.Commands.Commands;
using Core.Commands.Outputs;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class InitializeOAuthCommandHandler(
    OAuthStateTokenService tokenService,
    OAuthServiceFactory factory
) : CommandHandler<InitializeOAuthCommand, OAuthUrlOutput>
{
    public Task<Result<OAuthUrlOutput>> Handle(InitializeOAuthCommand cmd, CancellationToken _)
    {
        var state = new OAuthState(cmd.Provider);
        var stateToken = tokenService.Create(state);

        var service = factory.CreateInstance(cmd.Provider);

        if (service is null)
        {
            return Task.FromResult<Result<OAuthUrlOutput>>(new InvalidOAuthProvider());
        }

        var url = service.GenerateUrlFor(stateToken);

        return Task.FromResult<Result<OAuthUrlOutput>>(new OAuthUrlOutput(url, state.Id));
    }
}
