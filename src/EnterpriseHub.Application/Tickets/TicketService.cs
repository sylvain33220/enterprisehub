/*
@file TicketService.cs
@description Service for managing tickets in the EnterpriseHub application.
@author Poteaux sylvain
@site https://www.studio-purple.com
@mail poteaux.sylvain@gmail.com
@date © 2026 EnterpriseHub
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full
@version 1.0 license information.
*/
using EnterpriseHub.Application.Tickets.Dto;
using EnterpriseHub.Application.Tickets.Ports;
using EnterpriseHub.Application.Projects.Ports;
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Domain.Enums;
using EnterpriseHub.Application.Common.Exceptions;

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

    public async Task<TicketDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
    var ticket = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Ticket '{id}' was not found.");
    return ToDto(ticket);
    }

   public async Task<TicketDto> CreateAsync(CreateTicketRequest req, CancellationToken ct)
{
    if (await _projects.GetByIdAsync(req.ProjectId, ct) is null)
         throw new NotFoundAppException($"Project '{req.ProjectId}' was not found.");

    if (!Enum.IsDefined(typeof(TicketPriority), req.Priority))
        throw new ValidationAppException(new Dictionary<string, string[]>
        {
            ["priority"] = new[] { "Invalid ticket priority." }
        });

    var ticket = new Ticket(req.ProjectId, req.Title, req.Description, req.Priority);
    await _repo.AddAsync(ticket, ct);

    return ToDto(ticket);
}

    public async Task<TicketDto> UpdateAsync(Guid id, UpdateTicketRequest req, CancellationToken ct)
    {
    var ticket = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Ticket '{id}' was not found.");
  
    ticket.Update(req.Title, req.Description);
        ticket.UpdateStatus(req.Status.ToString());
        ticket.AssingnTo(req.AssignedToUserId);

        await _repo.UpdateAsync(ticket, ct);
        return ToDto(ticket);
    }

    public async Task  DeleteAsync(Guid id, CancellationToken ct)
    {
        var ticket = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Ticket '{id}' was not found.");
      
        await _repo.DeleteAsync(ticket, ct);
    }

    private static TicketDto ToDto(Ticket t)
        => new(t.Id, t.Title, t.Description, t.Priority, t.Status, t.AssignedToUserId, t.ProjectId );
}
