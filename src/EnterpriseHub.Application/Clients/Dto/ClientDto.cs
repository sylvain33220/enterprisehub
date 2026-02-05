namespace EnterpriseHub.Application.Clients.Dto;

public record ClientDto(
    Guid Id,
    string Name,
    string? Email,
    string? Phone
);
