namespace EnterpriseHub.Application.Auth.Dto;

public sealed record AuthToken(string AccessToken, string RefreshToken);