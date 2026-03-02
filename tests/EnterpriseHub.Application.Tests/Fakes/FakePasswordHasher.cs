using EnterpriseHub.Application.Auth.Ports;

namespace EnterpriseHub.Application.Tests.Fakes;

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hash:{password}";
    public bool Verify(string password, string passwordHash) => passwordHash == $"hash:{password}";
}