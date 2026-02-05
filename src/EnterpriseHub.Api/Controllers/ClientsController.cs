using EnterpriseHub.Application.Clients;
using EnterpriseHub.Application.Clients.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("clients")]
[Authorize] // on protège: besoin token
public class ClientsController : ControllerBase
{
    private readonly ClientService _svc;
    public ClientsController(ClientService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientRequest req, CancellationToken ct)
    {
        try
        {
            var created = await _svc.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, [FromBody] UpdateClientRequest req, CancellationToken ct)
    {
        var updated = await _svc.UpdateAsync(id, req, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
