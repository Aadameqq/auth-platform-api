using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;
using Core.Queries.Queries;

namespace Core.Queries;

public class GetTokenPayloadQueryHandler(
    AuthTokenService authTokenService,
    RevokedTokensRepository revokedTokensRepository
) : QueryHandler<GetTokenPayloadQuery, AccessTokenPayload>
{
    public async Task<Result<AccessTokenPayload>> Handle(
        GetTokenPayloadQuery query,
        CancellationToken _
    )
    {
        var payload = await authTokenService.FetchPayloadIfValid(query.AccessToken);

        if (payload is null || await revokedTokensRepository.HasBeenRevoked(query.AccessToken))
        {
            return new InvalidToken();
        }

        return payload;
    }
}
