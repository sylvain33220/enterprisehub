using EnterpriseHub.Tests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
namespace EnterpriseHub.Tests.Api;

public class EnterpriseHubApiFactory : WebApplicationFactory<Program>
{
    static EnterpriseHubApiFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        Environment.SetEnvironmentVariable("Database__Provider", "postgres");
        Environment.SetEnvironmentVariable("Database__AutoMigrate", "false");
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Default",
            "Host=127.0.0.1;Port=5432;Database=enterprisehub;Username=postgres;Password=postgres"
        );

        Environment.SetEnvironmentVariable("Jwt__Issuer", "enterprisehub-tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "enterprisehub-tests");
        Environment.SetEnvironmentVariable("Jwt__Key", "test_super_secret_key_12345678901234567890");
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var factory = WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });
            });
        });

        return factory.CreateClient();
    }
}