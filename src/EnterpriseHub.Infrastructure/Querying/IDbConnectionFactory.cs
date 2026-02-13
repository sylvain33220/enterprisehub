using System.Data;

namespace EnterpriseHub.Infrastructure.Querying;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
