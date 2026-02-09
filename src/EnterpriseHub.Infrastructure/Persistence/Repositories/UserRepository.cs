/*
@file UserRepository.cs
@description Repository implementation for managing users in the EnterpriseHub application using Entity Framework Core.
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
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