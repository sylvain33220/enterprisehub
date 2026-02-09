namespace EnterpriseHub.Application.Projects.Dto;

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    decimal? Budget,
    Guid ClientId
);