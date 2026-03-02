using EnterpriseHub.Application.Auth.Ports;
using EnterpriseHub.Application.Common.Interfaces;
using MediatR;

namespace EnterpriseHub.Application.Auth.Commands.Revoke;

public sealed class RevokeHandler : IRequestHandler<RevokeCommand>
{
    private readonly IEnterpriseHubDbContext _db;
    private readonly IRefreshTokenService _refresh;

    public RevokeHandler(IEnterpriseHubDbContext db, IRefreshTokenService refresh)
    {
        _db = db;
        _refresh = refresh;
    }

    public async Task Handle(RevokeCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return;

        var hash = _refresh.HashToken(request.RefreshToken);
        var existing = await _db.FindRefreshTokenByHashAsync(hash, ct);

        if (existing is null) return;

        if (existing.RevokedAt is null)
        {
            existing.Revoke(ReplacedByTokenHash: null);
            await _db.SaveChangesAsync(ct);
        }
    }
}