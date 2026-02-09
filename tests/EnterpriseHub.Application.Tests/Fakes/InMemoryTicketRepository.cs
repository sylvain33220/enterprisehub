using EnterpriseHub.Application.Tickets.Ports;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Tests.Fakes;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _items = new();

    public Task<List<Ticket>> GetAllAsync(CancellationToken ct)
        => Task.FromResult(_items.ToList());

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task AddAsync(Ticket ticket, CancellationToken ct)
    {
        _items.Add(ticket);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket ticket, CancellationToken ct)
        => Task.CompletedTask;

    public Task DeleteAsync(Ticket ticket, CancellationToken ct)
    {
        _items.Remove(ticket);
        return Task.CompletedTask;
    }

    public void Seed(Ticket ticket) => _items.Add(ticket);
}
