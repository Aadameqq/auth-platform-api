using System.Data;
using Core.Ports;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence.Dapper;

public class DapperSqlConnectionFactory(IOptions<DatabaseOptions> dbOptions) : SqlConnectionFactory
{
    private NpgsqlConnection? connection;

    public IDbConnection GetConnection()
    {
        if (connection is null || connection.State != ConnectionState.Open)
        {
            connection = new NpgsqlConnection(dbOptions.Value.ConnectionString);
            connection.Open();
        }

        return connection;
    }

    public void Dispose()
    {
        if (connection is { State: ConnectionState.Open })
        {
            connection.Dispose();
        }
    }
}
