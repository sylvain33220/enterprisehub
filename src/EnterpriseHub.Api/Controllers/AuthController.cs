using EnterpriseHub.Application.Auth;
using EnterpriseHub.Application.Auth.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req, CancellationToken ct)
    {
        var res = await _auth.RegisterAsync(req, ct);
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
    {
        var res = await _auth.LoginAsync(req, ct);
        return Ok(res);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { userId, email, role });
    }
}
