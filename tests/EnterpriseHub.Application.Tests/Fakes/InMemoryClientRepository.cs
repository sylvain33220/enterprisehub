/*
@file InMemoryClientRepository.cs
@description In-memory implementation of the IClientRepository interface for testing purposes in the EnterpriseHub application
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Application.Clients.Ports;

namespace EnterpriseHub.Application.Tests.Fakes;

public class InMemoryClientRepository : IClientRepository
{
    private readonly List<Client> _items = new();

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        => Task.FromResult(_items.Any(x => x.Email == email));

    public Task<List<Client>> GetAllAsync(CancellationToken ct)
        => Task.FromResult(_items.ToList());

    public Task<Client?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task AddAsync(Client client, CancellationToken ct)
    {
        _items.Add(client);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Client client, CancellationToken ct)
        => Task.CompletedTask;

    public Task DeleteAsync(Client client, CancellationToken ct)
    {
        _items.Remove(client);
        return Task.CompletedTask;
    }

    public void Seed(Client client) => _items.Add(client);
}