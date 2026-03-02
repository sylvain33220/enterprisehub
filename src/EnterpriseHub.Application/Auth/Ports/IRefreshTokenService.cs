namespace EnterpriseHub.Application.Auth.Ports;

public interface IRefreshTokenService
{
    string GenerateToken();
    string HashToken(string token);
}