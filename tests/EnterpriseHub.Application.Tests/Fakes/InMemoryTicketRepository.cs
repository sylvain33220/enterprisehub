/*
@file InMemoryTicketRepository.cs
@description In-memory implementation of the ITicketRepository interface for testing purposes in the EnterpriseHub application
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.    
@version 1.0
*/
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
