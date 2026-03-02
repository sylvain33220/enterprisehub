namespace EnterpriseHub.Application.Auth.Dto;

public record AuthResponse(
    string AccessToken,
    string? RefreshToken,
    object User
)
{
  public AuthResponse(string token, object value)
      : this(token, null, value)
  {
    Token = token;
    Value = value;
  }

  public string? Token { get; }
  public object? Value { get; }
}

public sealed record AuthTokens(
    string AccessToken,
    string RefreshToken
);
