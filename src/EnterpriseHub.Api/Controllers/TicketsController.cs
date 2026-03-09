/*
@author: Poteaux sylvain
@file: TicketsController.cs
@description: Controller for managing ticket-related endpoints, including CRUD operations. It uses authorization to protect the endpoints, ensuring that only authenticated users can access them. The controller interacts with the TicketService to perform the necessary operations and returns appropriate HTTP responses based on the outcome of each request.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@mail :poteaux.sylvain@gmail.com
@license: MIT
*/
using EnterpriseHub.Application.Tickets;
using EnterpriseHub.Application.Tickets.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;   
namespace EnterpriseHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
        return Ok(item);
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
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
         await _svc.DeleteAsync(id, ct);
        return NoContent();
        
    }
}
