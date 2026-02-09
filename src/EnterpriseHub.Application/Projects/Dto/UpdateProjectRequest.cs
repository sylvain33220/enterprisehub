namespace EnterpriseHub.Application.Projects.Dto;

public record UpdateProjectRequest(
    string Name,
    string? Description
);
