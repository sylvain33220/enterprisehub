using EnterpriseHub.Application.Auth.Dto;
using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Application.Common.Exceptions;
using EnterpriseHub.Application.Common.Interfaces;
using EnterpriseHub.Domain.Entities;
using MediatR;

namespace EnterpriseHub.Application.Auth.Commands.Refresh;

public sealed class RefreshHandler : IRequestHandler<RefreshCommand, AuthTokens>
{
    private readonly IEnterpriseHubDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenService _refresh;

    public RefreshHandler(IEnterpriseHubDbContext db, IJwtTokenGenerator jwt, IRefreshTokenService refresh)
    {
        _db = db;
        _jwt = jwt;
        _refresh = refresh;
    }

    public async Task<AuthTokens> Handle(RefreshCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new UnauthorizedAppException("Invalid credentials.");

        var incomingHash = _refresh.HashToken(request.RefreshToken);

        var existing = await _db.FindRefreshTokenByHashAsync(incomingHash, ct);
        if (existing is null || !existing.IsActive)
            throw new UnauthorizedAppException("Invalid credentials.");

        var user = await _db.GetUserByIdAsync(existing.UserId, ct);
        if (user is null)
            throw new UnauthorizedAppException("Invalid credentials.");

        // ✅ rotation
        var newRefreshToken = _refresh.GenerateToken();
        var newHash = _refresh.HashToken(newRefreshToken);

        existing.Revoke(ReplacedByTokenHash: newHash);

        var newRt = new RefreshToken(
            userId: user.Id,
            tokenHash: newHash,
            expiresAt: DateTime.UtcNow.AddDays(14),
            ip: request.Ip,
            userAgent: request.UserAgent
        );

        _db.AddRefreshToken(newRt);

        var newAccess = _jwt.GenerateToken(user);

        await _db.SaveChangesAsync(ct);

        return new AuthTokens(newAccess, newRefreshToken);
    }
}