namespace EnterpriseHub.Application.Clients.Dto;

public record UpdateClientRequest(
    string Name,
    string? Email,
    string? Phone
);
