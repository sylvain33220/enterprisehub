using System.Security.Cryptography;
using System.Text;
using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Infrastructure.Auth;

public sealed class RefreshTokenService : IRefreshTokenService, IJwtTokenGenerator
{
  public string GenerateToken()
  {
    var bytes = RandomNumberGenerator.GetBytes(64);
    return Convert.ToBase64String(bytes);
  }

  public string GenerateToken(User user)
  {
    throw new NotImplementedException();
  }

  public string HashToken(string token)
  {
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    return Convert.ToBase64String(hash);
  }
} 
