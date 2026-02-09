using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EnterpriseHub.Infrastructure.Persistence;

public class EnterpriseHubDbContextFactory : IDesignTimeDbContextFactory<EnterpriseHubDbContext>
{
    public EnterpriseHubDbContext CreateDbContext(string[] args)
    {
        // Connection string hard-coded DEV (ok pour design-time). On la centralisera ensuite.
        var cs = Environment.GetEnvironmentVariable("EH_CONNECTIONSTRING")
         ?? "Host=localhost;Port=5432;Database=enterprisehub;Username=postgres;Password=postgres";


        var optionsBuilder = new DbContextOptionsBuilder<EnterpriseHubDbContext>();
        optionsBuilder.UseNpgsql(cs);

        return new EnterpriseHubDbContext(optionsBuilder.Options);
    }
}
