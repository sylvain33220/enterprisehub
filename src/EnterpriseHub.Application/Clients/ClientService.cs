using EnterpriseHub.Application.Clients.Dto;
using EnterpriseHub.Application.Clients.Ports;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Clients;

public class ClientService
{
    private readonly IClientRepository _repo;

    public ClientService(IClientRepository repo) => _repo = repo;

    public async Task<List<ClientDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(ToDto).ToList();
    }

    public async Task<ClientDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct);
        return client is null ? null : ToDto(client);
    }

    public async Task<ClientDto> CreateAsync(CreateClientRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
            throw new ArgumentException("Name is required.");

        var email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim().ToLowerInvariant();

        if (email is not null)
        {
            var exists = await _repo.ExistsByEmailAsync(email, ct);
            if (exists) throw new InvalidOperationException("Client email already exists.");
        }
        var client = new Client(req.Name, req.Email, req.Phone);

        await _repo.AddAsync(client, ct);

        return ToDto(client);
    }

    public async Task<ClientDto?> UpdateAsync(Guid id, UpdateClientRequest req, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct);
        if (client is null) return null;

        client.Update(req.Name, req.Email, req.Phone);
        await _repo.UpdateAsync(client, ct);

        return ToDto(client);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var client = await _repo.GetByIdAsync(id, ct);
        if (client is null) return false;

        await _repo.DeleteAsync(client, ct);
        return true;
    }

    private static ClientDto ToDto(Client c)
        => new(c.Id, c.Name, c.Email, c.Phone);
}
