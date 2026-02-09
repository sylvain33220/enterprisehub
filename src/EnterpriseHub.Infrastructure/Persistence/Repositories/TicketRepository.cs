using EnterpriseHub.Application.Tickets.Ports;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly EnterpriseHubDbContext _db;
    public TicketRepository(EnterpriseHubDbContext db) => _db = db;

    public Task<List<Ticket>> GetAllAsync(CancellationToken ct)
        => _db.Tickets.AsNoTracking().ToListAsync(ct);

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task AddAsync(Ticket ticket, CancellationToken ct)
    {
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken ct)
    {
        _db.Tickets.Update(ticket);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Ticket ticket, CancellationToken ct)
    {
        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync(ct);
    }
}
