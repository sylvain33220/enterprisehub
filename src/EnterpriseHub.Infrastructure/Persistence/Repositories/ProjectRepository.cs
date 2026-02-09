/*
@file ProjectRepository.cs
@description Repository implementation for managing projects in the EnterpriseHub application using Entity Framework Core.
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
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
