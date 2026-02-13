using EnterpriseHub.Application.Dashboard.Dtos;

namespace EnterpriseHub.Application.Dashboard.Ports;

public interface IDashboardReadRepository
{
    Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct);

    Task<IReadOnlyList<TicketsByStatusDto>> GetTicketsByStatusAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct
    );

    Task<IReadOnlyList<TopClientDto>> GetTopClientsAsync(
        int limit,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct
    );
}
