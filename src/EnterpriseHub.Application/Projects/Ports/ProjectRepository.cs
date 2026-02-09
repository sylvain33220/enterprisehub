using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Projects.Ports;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(CancellationToken ct);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Project project, CancellationToken ct);
    Task UpdateAsync(Project project, CancellationToken ct);
    Task DeleteAsync(Project project, CancellationToken ct);
}
