using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EnterpriseHub.Infrastructure.Querying;

public sealed class NpgsqlConnectionFactory(IConfiguration config) : IDbConnectionFactory
{
  public IDbConnection CreateConnection()
  {
    var cs = config.GetConnectionString("Default");
    if(string.IsNullOrWhiteSpace(cs))
    {
      
        throw new InvalidOperationException("Missing ConnectionStrings:Default");
    }
        return new NpgsqlConnection(cs);
    
  }
}