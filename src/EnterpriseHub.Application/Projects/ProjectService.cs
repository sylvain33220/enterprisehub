using EnterpriseHub.Application.Projects.Dto;
using EnterpriseHub.Application.Projects.Ports;
using EnterpriseHub.Application.Clients.Ports;
using EnterpriseHub.Domain.Entities;

namespace EnterpriseHub.Application.Projects;

public class ProjectService
{
    private readonly IProjectRepository _repo;
    private readonly IClientRepository _clients;

    public ProjectService(IProjectRepository repo, IClientRepository clients)
    {
        _repo = repo;
        _clients = clients;
    }

    public async Task<List<ProjectDto>> GetAllAsync(CancellationToken ct)
        => [.. (await _repo.GetAllAsync(ct)).Select(ToDto)];

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        return project is null ? null : ToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest req, CancellationToken ct)
    {
        if (await _clients.GetByIdAsync(req.ClientId, ct) is null)
            throw new KeyNotFoundException("Client not found.");

        var project = new Project(req.ClientId, req.Name, req.Description );
        await _repo.AddAsync(project, ct);

        return ToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest req, CancellationToken ct)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        if (project is null) return null;

        project.Update(req.Name, req.Description);
        await _repo.UpdateAsync(project, ct);

        return ToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var project = await _repo.GetByIdAsync(id, ct);
        if (project is null) return false;

        await _repo.DeleteAsync(project, ct);
        return true;
    }

    private static ProjectDto ToDto(Project p)
        => new(p.Id, p.Name, p.Description, p.Status.ToString(), p.Budget, p.ClientId);
}
