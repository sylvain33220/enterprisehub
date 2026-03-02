using EnterpriseHub.Application.Auth;
using EnterpriseHub.Application.Auth.Commands.Refresh;
using EnterpriseHub.Application.Auth.Commands.Revoke;
using EnterpriseHub.Application.Auth.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using EnterpriseHub.Application.Auth.Commands.Login;
using Microsoft.Extensions.Options;
using EnterpriseHub.Api.Auth;
using EnterpriseHub.Application.Common.Exceptions;
namespace EnterpriseHub.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    private readonly IMediator _mediator;
    private readonly CookieConfig _cookieCfg;

private static SameSiteMode ParseSameSite(string value) =>
    value?.ToLowerInvariant() switch
    {
        "strict" => SameSiteMode.Strict,
        "none" => SameSiteMode.None,
        _ => SameSiteMode.Lax
    };
    public AuthController(
    AuthService auth,
    IMediator mediator,
    IOptions<CookieConfig> cookieOptions)
{
    _auth = auth;
    _mediator = mediator;
    _cookieCfg = cookieOptions.Value;
}
        private void SetRefreshCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _cookieCfg.Secure,
                SameSite = ParseSameSite(_cookieCfg.SameSite),
                Expires = DateTime.UtcNow.AddDays(_cookieCfg.RefreshTokenExpiryDays),
                Path = "/auth/refresh"
            };
    
            Response.Cookies.Append(_cookieCfg.RefreshCookieName, refreshToken, cookieOptions);
        }

        private void DeleteRefreshCookie()
        {
            Response.Cookies.Delete(_cookieCfg.RefreshCookieName, new CookieOptions { Path = "/auth/refresh" });
        }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req, CancellationToken ct)
    {
        var res = await _auth.RegisterAsync(req, ct);
        return Ok(res);
    }

    [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var cmd = new LoginCommand
            {
                Email = req.Email,
                Password = req.Password,
                Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            var tokens = await _mediator.Send(cmd, ct);
            SetRefreshCookie(tokens.RefreshToken);
            return Ok(new { accessToken = tokens.AccessToken });
        }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { userId, email, role });
    }
    [HttpPost("refresh")]
public async Task<IActionResult> Refresh(CancellationToken ct)
{
    var rt = Request.Cookies[_cookieCfg.RefreshCookieName];
    if (string.IsNullOrWhiteSpace(rt))
    {
        return Unauthorized(new ProblemDetails
        {
            Title = "Unauthorized",
            Detail = "Missing refresh cookie.",
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://httpstatuses.com/401"
        });
    }

    var cmd = new RefreshCommand
    {
        RefreshToken = rt,
        Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = Request.Headers.UserAgent.ToString()
    };

    var tokens = await _mediator.Send(cmd, ct);

    SetRefreshCookie(tokens.RefreshToken);

    return Ok(new { accessToken = tokens.AccessToken });
}

    [HttpPost("revoke")]
public async Task<IActionResult> Revoke(CancellationToken ct)
{
    var rt = Request.Cookies[_cookieCfg.RefreshCookieName];
    if (string.IsNullOrWhiteSpace(rt))
        throw new UnauthorizedException("Missing refresh cookie.");
         await _mediator.Send(new RevokeCommand { RefreshToken = rt }, ct);

    DeleteRefreshCookie();
    return NoContent();
}

}
