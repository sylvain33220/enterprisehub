using EnterpriseHub.Application.Tickets.Dto;
using EnterpriseHub.Application.Tickets.Ports;
using EnterpriseHub.Application.Projects.Ports;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Tickets;

public class TicketService
{
    private readonly ITicketRepository _repo;
    private readonly IProjectRepository _projects;

    public TicketService(ITicketRepository repo, IProjectRepository projects)
    {
        _repo = repo;
        _projects = projects;
    }

    public async Task<List<TicketDto>> GetAllAsync(CancellationToken ct)
        => (await _repo.GetAllAsync(ct)).Select(ToDto).ToList();

    public async Task<TicketDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var ticket = await _repo.GetByIdAsync(id, ct);
        return ticket is null ? null : ToDto(ticket);
    }

    public async Task<TicketDto> CreateAsync(CreateTicketRequest req, CancellationToken ct)
    {
        if (await _projects.GetByIdAsync(req.ProjectId, ct) is null)
            throw new KeyNotFoundException("Project not found.");

        var priority = Enum.TryParse<EnterpriseHub.Domain.Enums.TicketPriority>(req.Description, true, out var parsedPriority)
            ? parsedPriority
            : throw new ArgumentException("Invalid ticket priority.", nameof(req.Description));
        var ticket = new Ticket(Guid.NewGuid(), req.ProjectId.ToString(), priority.ToString());
        await _repo.AddAsync(ticket, ct);

        return ToDto(ticket);
    }

    public async Task<TicketDto?> UpdateAsync(Guid id, UpdateTicketRequest req, CancellationToken ct)
    {
        var ticket = await _repo.GetByIdAsync(id, ct);
        if (ticket is null) return null;

        ticket.Update(req.Title, req.Description);
        ticket.UpdateStatus(req.Status.ToString());

        await _repo.UpdateAsync(ticket, ct);
        return ToDto(ticket);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var ticket = await _repo.GetByIdAsync(id, ct);
        if (ticket is null) return false;

        await _repo.DeleteAsync(ticket, ct);
        return true;
    }

    private static TicketDto ToDto(Ticket t)
        => new(t.Id, t.Title, t.Description, t.Priority, t.Status, t.ProjectId);
}
