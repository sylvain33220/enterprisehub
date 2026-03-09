/*
@author: Poteaux sylvain
@file: HealthController.cs
@description: Controller for health check endpoints, providing a simple way to verify the application's health status, including database connectivity.
@version: 1.0
@date: 2026.03
@site: https://studio-purple.com
@license: MIT
*/
using EnterpriseHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("health-controller")]
public class HealthController : ControllerBase
{
    private readonly EnterpriseHubDbContext _db;

    public HealthController(EnterpriseHubDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var canConnect = await _db.Database.CanConnectAsync();
        return Ok(new { status = "ok", db = canConnect ? "up" : "down" });
    }
}
