using EnterpriseHub.Application.Auth.Dto;
using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Application.Common.Exceptions;
using EnterpriseHub.Application.Common.Interfaces;
using EnterpriseHub.Domain.Entities;
using MediatR;

namespace EnterpriseHub.Application.Auth.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthTokens>
{
    private readonly IEnterpriseHubDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenService _refresh;
    private readonly IPasswordHasher _passwords;

    public LoginHandler(
        IEnterpriseHubDbContext db,
        IJwtTokenGenerator jwt,
        IRefreshTokenService refresh,
        IPasswordHasher passwords)
    {
        _db = db;
        _jwt = jwt;
        _refresh = refresh;
        _passwords = passwords;
    }

    public async Task<AuthTokens> Handle(LoginCommand request, CancellationToken ct)
    {
        var email = (request.Email ?? "").Trim().ToLowerInvariant();

        var user = await _db.FindUserByEmailAsync(email, ct);
        if (user is null) throw new UnauthorizedException("Invalid credentials.");

        if (!_passwords.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var access = _jwt.GenerateToken(user);

        // ⚠️ On unifie le nom de méthode
        var refreshToken = _refresh.GenerateToken();
        var refreshHash = _refresh.HashToken(refreshToken);

        var rt = new RefreshToken(
            userId: user.Id,
            tokenHash: refreshHash,
            expiresAt: DateTime.UtcNow.AddDays(14),
            ip: request.Ip,
            userAgent: request.UserAgent
        );

        _db.AddRefreshToken(rt);
        await _db.SaveChangesAsync(ct);

        return new AuthTokens(access, refreshToken);
    }
}