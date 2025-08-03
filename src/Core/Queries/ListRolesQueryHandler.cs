using Core.Domain;
using Core.Queries.Queries;

namespace Core.Queries;

public class ListRolesQueryHandler : QueryHandler<ListRolesQuery, List<Role>>
{
    public Task<Result<List<Role>>> Handle(ListRolesQuery query, CancellationToken _)
    {
        var roles = Enum.GetValues<Role>().ToList();

        return Task.FromResult<Result<List<Role>>>(roles);
    }
}
