using Core.Domain;
using Core.Dtos;
using Core.Exceptions;
using Core.Ports;
using Dapper;

namespace Core.UseCases;

public class GetCurrentAccountUseCase(SqlConnectionFactory connectionFactory)
{
    public async Task<Result<AccountDto>> Execute(Guid id)
    {
        var sql = """select "Id", "UserName", "Email" from "Accounts" where "Id" = @Id""";

        var connection = connectionFactory.GetConnection();

        var found = await connection.QuerySingleAsync<AccountDto>(sql, new { Id = id });

        if (found is null)
        {
            return new NoSuch<Account>();
        }

        return found;
    }
}
