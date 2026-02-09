using EnterpriseHub.Application.Projects.Ports;
using EnterpriseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly EnterpriseHubDbContext _db;
    public ProjectRepository(EnterpriseHubDbContext db) => _db = db;

    public Task<List<Project>> GetAllAsync(CancellationToken ct)
        => _db.Projects.AsNoTracking().ToListAsync(ct);

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Projects.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Project project, CancellationToken ct)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Project project, CancellationToken ct)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Project project, CancellationToken ct)
    {
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(ct);
    }
}
