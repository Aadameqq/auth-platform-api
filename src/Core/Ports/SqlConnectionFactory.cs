using System.Data;

namespace Core.Ports;

public interface SqlConnectionFactory
{
    IDbConnection GetConnection();
}
