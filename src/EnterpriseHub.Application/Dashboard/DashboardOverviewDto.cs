namespace EnterpriseHub.Application.Dashboard.Dtos;

public sealed record DashboardOverviewDto(
    int TotalClients,
    int TotalProjects,
    int TotalTickets
);
