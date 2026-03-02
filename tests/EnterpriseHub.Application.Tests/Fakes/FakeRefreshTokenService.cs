using EnterpriseHub.Application.Auth.Ports;

namespace EnterpriseHub.Application.Tests.Fakes;

public sealed class FakeRefreshTokenService : IRefreshTokenService
{
    private int _i = 0;

    public string GenerateToken() => $"rt-{++_i}";
    public string HashToken(string token) => $"hash-{token}";
}