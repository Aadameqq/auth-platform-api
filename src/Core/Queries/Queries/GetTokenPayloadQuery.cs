using Core.Dtos;

namespace Core.Queries.Queries;

public record GetTokenPayloadQuery(string AccessToken) : Query<AccessTokenPayload>;
