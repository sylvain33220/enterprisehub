namespace EnterpriseHub.Application.Auth.Dto;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
