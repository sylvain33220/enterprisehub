/*
* @file  : ProjectController.cs
* @brief : Controller for managing projects, including CRUD operations.
* @date  : 2024-06-01
* @author : Poteaux sylvain
*/
using EnterpriseHub.Application.Projects;
using EnterpriseHub.Application.Projects.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
namespace EnterpriseHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _svc;
    public ProjectsController(ProjectService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _svc.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectRequest req, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, UpdateProjectRequest req, CancellationToken ct)
    {
        var updated = await _svc.UpdateAsync(id, req, ct);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct); 
        return NoContent();
    }
}
