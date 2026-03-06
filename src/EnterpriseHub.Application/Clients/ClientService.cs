/*
@file ClientService.cs
@description Service for managing clients in the EnterpriseHub application.
@author Poteaux sylvain
@site https://www.studio-purple.com
@mail poteaux.sylvain@gmail.com
 * © 2026 EnterpriseHub
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Application.Clients.Dto;
using EnterpriseHub.Application.Clients.Ports;
using EnterpriseHub.Application.Common.Exceptions;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Clients;

public class ClientService
{
    private readonly IClientRepository _repo;

    public ClientService(IClientRepository repo) => _repo = repo;

    public async Task<List<ClientDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return [.. items.Select(ToDto)];
    }

    public async Task<ClientDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Client {id} was not found.");
        return ToDto(client);
        
    }

    public async Task<ClientDto> CreateAsync(CreateClientRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ValidationAppException("Client name is required.");

        var email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim().ToLowerInvariant();

        if (email is not null)
        {
            var exists = await _repo.ExistsByEmailAsync(email, ct);
            if (exists) throw new ConflictAppException("A client with the same email already exists.");
        }
        var client = new Client(req.Name, req.Email, req.Phone);

        await _repo.AddAsync(client, ct);

        return ToDto(client);
    }

    public async Task<ClientDto> UpdateAsync(Guid id, UpdateClientRequest req, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Client {id} not found.");
       

        client.Update(req.Name, req.Email, req.Phone);
        await _repo.UpdateAsync(client, ct);

        return ToDto(client);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct) ?? throw new NotFoundAppException($"Client {id} not found.");
       
        await _repo.DeleteAsync(client, ct);
    }

    private static ClientDto ToDto(Client c)
        => new(c.Id, c.Name, c.Email, c.Phone);
}
