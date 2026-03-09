/*
@author: Poteaux sylvain
@file: ClientsController.cs
@description: Controller for managing client-related endpoints, including CRUD operations. It uses authorization to protect the endpoints, ensuring that only authenticated users can access them. The controller interacts with the ClientService to perform the necessary operations and returns appropriate HTTP responses based on the outcome of each request.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@license: MIT
*/
using EnterpriseHub.Application.Clients;
using EnterpriseHub.Application.Clients.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
namespace EnterpriseHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize] // on protège: besoin token pour accéder à ces endpoints
public class ClientsController : ControllerBase
{
    private readonly ClientService _svc;
    public ClientsController(ClientService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll(CancellationToken ct)
    => Ok(await _svc.GetAllAsync(ct));
    // {
    //     return  Ok(await Task.FromResult(new { message = "Clients enpoints - GET all" }));
        
    // }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _svc.GetByIdAsync(id, ct));    

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientRequest req, CancellationToken ct)
    {
            var created = await _svc.CreateAsync(req, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, version="1.0" }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientDto>> Update(Guid id, [FromBody] UpdateClientRequest req, CancellationToken ct)
        => Ok(await _svc.UpdateAsync(id, req, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
