using Core.Domain;

namespace Core.Queries;

public class ListRolesQueryHandler
{
    public Result<List<Role>> Execute()
    {
        return Role.Roles;
    }
}
