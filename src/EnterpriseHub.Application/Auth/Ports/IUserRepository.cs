using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Auth.Ports;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email , CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);

}