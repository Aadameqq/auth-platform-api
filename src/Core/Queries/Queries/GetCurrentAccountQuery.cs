using Core.Queries.Outputs;

namespace Core.Queries.Queries;

public record GetCurrentAccountQuery(Guid Id) : Query<AccountDetailsOutput>;
