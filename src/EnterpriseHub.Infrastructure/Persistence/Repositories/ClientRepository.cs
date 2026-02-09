/*
@file ClientRepository.cs
@description Repository implementation for managing clients in the EnterpriseHub application using Entity Framework Core.
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Application.Clients.Ports;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly EnterpriseHubDbContext _db;
    public ClientRepository(EnterpriseHubDbContext db) => _db = db;

    public Task<List<Client>> GetAllAsync(CancellationToken ct = default)
        => _db.Clients.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Client client, CancellationToken ct = default)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Client client, CancellationToken ct = default)
    {
        _db.Clients.Update(client);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Client client, CancellationToken ct = default)
    {
        client.Deactivate();
        await _db.SaveChangesAsync(ct);
    }
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    => _db.Clients.AnyAsync(c => c.IsActive && c.Email == email, ct);

}
