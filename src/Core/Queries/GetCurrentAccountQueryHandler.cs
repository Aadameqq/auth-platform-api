using Core.Domain;
using Core.Exceptions;
using Core.Ports;
using Core.Queries.Outputs;
using Dapper;

namespace Core.Queries;

public class GetCurrentAccountQueryHandler(SqlConnectionFactory connectionFactory)
{
    public async Task<Result<AccountDetailsOutput>> Execute(Guid id)
    {
        var sql = """select "Id", "UserName", "Email" from "Accounts" where "Id" = @Id""";

        var connection = connectionFactory.GetConnection();

        var found = await connection.QuerySingleAsync<AccountDetailsOutput>(sql, new { Id = id });

        if (found is null)
        {
            return new NoSuch<Account>();
        }

        return found;
    }
}
