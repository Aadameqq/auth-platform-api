using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;
using Core.Queries.Queries;

namespace Core.Queries;

public class GetTokenPayloadQueryHandler(AuthTokenService authTokenService)
    : QueryHandler<GetTokenPayloadQuery, AccessTokenPayload>
{
    public async Task<Result<AccessTokenPayload>> Handle(
        GetTokenPayloadQuery query,
        CancellationToken _
    )
    {
        var payload = await authTokenService.FetchPayloadIfValid(query.AccessToken);

        if (payload is null)
        {
            return new InvalidToken();
        }

        return payload;
    }
}
