using Core.Commands.Commands;
using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;

namespace Core.Commands;

public class AuthorizeCommandHandler(
    RevokedTokensRepository revokedTokensRepository,
    AuthTokenService authTokenService
) : CommandHandler<AuthorizeCommand, AccessTokenPayload>
{
    public async Task<Result<AccessTokenPayload>> Handle(AuthorizeCommand cmd, CancellationToken _)
    {
        var payload = await authTokenService.FetchPayloadIfValid(cmd.AccessToken);

        if (payload is null || await revokedTokensRepository.HasBeenRevoked(cmd.AccessToken))
        {
            return new InvalidToken();
        }

        return payload;
    }
}
