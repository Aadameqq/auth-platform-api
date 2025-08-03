using Core.Domain;
using Core.Ports;
using Core.Queries.Queries;
using Dapper;

namespace Core.Queries;

public class HasAccountBeenActivatedQueryHandler(SqlConnectionFactory connectionFactory)
    : QueryHandler<HasAccountBeenActivatedQuery, bool>
{
    public async Task<Result<bool>> Handle(HasAccountBeenActivatedQuery query, CancellationToken _)
    {
        var sql = """select "Activated" from "Accounts" where "Id" = @Id""";

        var connection = connectionFactory.GetConnection();

        var isActivated = await connection.QuerySingleAsync<bool>(sql, new { query.Id });

        return isActivated;
    }
}
