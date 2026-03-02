using Microsoft.AspNetCore.Mvc.Testing;

namespace EnterpriseHub.Tests.Api;

public sealed class EnterpriseHubApiFactory : WebApplicationFactory<Program>
{
    // Pour l’instant on laisse tel quel (tu peux overrider la config/DB plus tard)
}