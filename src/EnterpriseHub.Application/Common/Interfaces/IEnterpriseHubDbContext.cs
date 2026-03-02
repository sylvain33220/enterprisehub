using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Common.Interfaces;

public interface IEnterpriseHubDbContext
{
    Task<User?> FindUserByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);

    Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken ct);
    void AddRefreshToken(RefreshToken token);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}