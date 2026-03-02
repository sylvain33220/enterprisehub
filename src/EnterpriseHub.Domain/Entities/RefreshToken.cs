using EnterpriseHub.Domain.Common;

namespace EnterpriseHub.Domain.Entities;

public sealed class RefreshToken : EntityBase
{
  public Guid UserId {get; private set;}
  public string TokenHash {get; private set;} = default!;
  public DateTime ExpiresAt {get; private set;}

  public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;
  public DateTime? RevokedAt {get; private set;}

  public string? ReplacedByTokenHash {get; private set;}

  public string? CreatedByIp {get; private set;}
  public string? RevokedByIp {get; private set;}

  public string? UserAgent {get; private set;}

  #pragma warning disable CS8618
  private RefreshToken() {}
  #pragma warning restore CS8618

   public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, string? ip, string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(tokenHash)) throw new ArgumentException("Token hash is required.");
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedByIp = ip;
        UserAgent = userAgent;
        Touch();
    }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public void Revoke(string? ReplacedByTokenHash)
  {
    RevokedAt = DateTime.UtcNow;
    this.ReplacedByTokenHash = ReplacedByTokenHash;
    Touch();
  }
}