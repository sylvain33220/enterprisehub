namespace EnterpriseHub.Application.Projects.Dto;

public record CreateProjectRequest(
    string Name,
    Guid ClientId,
    string? Description,
    decimal? Budget
);
