/*
@author: Poteaux sylvain
@file: DashboardController.cs
@description: Controller for dashboard-related endpoints, providing aggregated data for the application's dashboard. It includes endpoints for retrieving an overview of key metrics, ticket counts by status, and top clients based on activity. The controller uses authorization to ensure that only authenticated users can access the dashboard data, and it interacts with the IDashboardReadRepository to fetch the necessary information from the data source.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@license: MIT
*/
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
