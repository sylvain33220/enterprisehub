namespace EnterpriseHub.Application.Clients.Dto;

public record CreateClientRequest(
    string Name,
    string? Email,
    string? Phone
);