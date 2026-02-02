using EnterpriseHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("health")]
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
