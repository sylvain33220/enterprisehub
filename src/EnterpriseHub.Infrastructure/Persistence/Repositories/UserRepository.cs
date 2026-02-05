using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly EnterpriseHubDbContext _db;

    public UserRepository(EnterpriseHubDbContext db)
        => _db = db;
    

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
    => await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }
}