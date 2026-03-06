using EnterpriseHub.Application.Dashboard.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
namespace EnterpriseHub.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public sealed class DashboardController(IDashboardReadRepository repo) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken ct)
        => Ok(await repo.GetOverviewAsync(ct));

    [HttpGet("tickets-by-status")]
    public async Task<IActionResult> TicketsByStatus([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, CancellationToken ct)
        => Ok(await repo.GetTicketsByStatusAsync(fromUtc, toUtc, ct));

    [HttpGet("top-clients")]
    public async Task<IActionResult> TopClients([FromQuery] int limit, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc, CancellationToken ct)
        => Ok(await repo.GetTopClientsAsync(limit, fromUtc, toUtc, ct));
}
