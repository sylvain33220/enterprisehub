using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Domain.Entities;
namespace EnterpriseHub.Application.Tests.Fakes;

public sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(User user) => $"access-{user.Id}";
    public string HashToken(string token) => $"hash-{token}";
}