using EnterpriseHub.Application.Auth.Ports;

namespace EnterpriseHub.Infrastructure.Auth;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
       => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string passwordHash)
       => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
