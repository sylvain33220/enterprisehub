using EnterpriseHub.Application.Tickets;
using EnterpriseHub.Application.Tickets.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly TicketService _svc;
    public TicketsController(TicketService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<TicketDto>>> GetAll(CancellationToken ct)
        => Ok(await _svc.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create(CreateTicketRequest req, CancellationToken ct)
    {
        var created = await _svc.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, UpdateTicketRequest req, CancellationToken ct)
    {
        var updated = await _svc.UpdateAsync(id, req, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
