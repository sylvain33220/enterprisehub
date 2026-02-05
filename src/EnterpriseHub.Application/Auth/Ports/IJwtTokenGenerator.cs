using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Auth.Ports;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}