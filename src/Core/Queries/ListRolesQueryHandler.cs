using Core.Domain;
using Core.Queries.Queries;

namespace Core.Queries;

public class ListRolesQueryHandler : QueryHandler<ListRolesQuery, List<Role>>
{
    public Task<Result<List<Role>>> Handle(ListRolesQuery query, CancellationToken _)
    {
        return Task.FromResult<Result<List<Role>>>(Role.Roles);
    }
}
