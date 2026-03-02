using EnterpriseHub.Application.Common.Interfaces;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Tests.Fakes;

public sealed class InMemoryEnterpriseHubDbContext : IEnterpriseHubDbContext
{
  public List<User> UserStore { get; } = new();
  public List<RefreshToken> RefreshTokenStore { get; } = new();

public Task<User?> FindUserByEmailAsync(string email, CancellationToken ct)
      => Task.FromResult(UserStore.SingleOrDefault(u => u.Email == email));
  public Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct)
      => Task.FromResult(UserStore.SingleOrDefault(u => u.Id == id));
    public Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken ct)
        => Task.FromResult(RefreshTokenStore.SingleOrDefault(t => t.TokenHash == tokenHash));

    public void AddRefreshToken(RefreshToken token) => RefreshTokenStore.Add(token);
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => Task.FromResult(1);

        //helpers
    public void SeedUser(User user) => UserStore.Add(user);
    public void SeedRefreshToken(RefreshToken token) => RefreshTokenStore.Add(token);
}