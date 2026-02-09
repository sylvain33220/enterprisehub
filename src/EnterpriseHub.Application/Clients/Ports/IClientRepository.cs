using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Clients.Ports;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync(CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Client client, CancellationToken ct = default);
    Task UpdateAsync(Client client, CancellationToken ct = default);
    Task DeleteAsync(Client client, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}
