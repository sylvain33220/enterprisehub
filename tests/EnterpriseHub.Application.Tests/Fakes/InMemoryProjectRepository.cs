/*
@file InMemoryProjectRepository.cs
@description In-memory implementation of the IProjectRepository interface for testing purposes in the EnterpriseHub application
@author Poteaux sylvain
@site https://www.studio-purple.com
@date 2026-09-02
@EnterpriseHub is licensed under the MIT License. See LICENSE file in the project root for full license information.
@version 1.0
*/
using EnterpriseHub.Domain.Entities;
using EnterpriseHub.Application.Projects.Ports;

namespace EnterpriseHub.Application.Tests.Fakes;

public class InMemoryProjectRepository : IProjectRepository
{
    private readonly List<Project> _items = new();

    public Task<List<Project>> GetAllAsync(CancellationToken ct)
        => Task.FromResult(_items.ToList());

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task AddAsync(Project project, CancellationToken ct)
    {
        _items.Add(project);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Project project, CancellationToken ct)
        => Task.CompletedTask;

    public Task DeleteAsync(Project project, CancellationToken ct)
    {
        _items.Remove(project);
        return Task.CompletedTask;
    }

    public void Seed(Project project) => _items.Add(project);
}