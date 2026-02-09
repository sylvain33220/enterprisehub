using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Tickets.Ports;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync(CancellationToken ct);
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Ticket ticket, CancellationToken ct);
    Task UpdateAsync(Ticket ticket, CancellationToken ct);
    Task DeleteAsync(Ticket ticket, CancellationToken ct);
}