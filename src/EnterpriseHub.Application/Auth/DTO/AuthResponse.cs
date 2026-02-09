namespace EnterpriseHub.Application.Auth.Dto;

public record AuthResponse(
    string AccessToken,
    object User 
);
