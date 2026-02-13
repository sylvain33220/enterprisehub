using Dapper;
using EnterpriseHub.Application.Dashboard.Dtos;
using EnterpriseHub.Application.Dashboard.Ports;
using System.Data;

namespace EnterpriseHub.Infrastructure.Querying;

public sealed class DashboardReadRepository(IDbConnectionFactory db) : IDashboardReadRepository
{
    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken ct)
    {
        const string sql = """
        SELECT
          (SELECT COUNT(*) FROM clients)::int   AS TotalClients,
          (SELECT COUNT(*) FROM projects)::int  AS TotalProjects,
          (SELECT COUNT(*) FROM tickets)::int   AS TotalTickets;
        """;

        using var conn = db.CreateConnection();
        return await conn.QuerySingleAsync<DashboardOverviewDto>(
            new CommandDefinition(sql, cancellationToken: ct)
        );
    }

    public async Task<IReadOnlyList<TicketsByStatusDto>> GetTicketsByStatusAsync(
    DateTime? fromUtc,
    DateTime? toUtc,
    CancellationToken ct
)
{
    var sql = """
    SELECT
      t.status::text AS status,
      COUNT(*)::int  AS count
    FROM tickets t
    WHERE 1=1
    """;

    var p = new DynamicParameters();

    if (fromUtc.HasValue)
    {
        sql += "\n  AND t.created_at_utc >= @from_utc";
        p.Add("from_utc", fromUtc.Value, DbType.DateTime);
    }

    if (toUtc.HasValue)
    {
        sql += "\n  AND t.created_at_utc < @to_utc";
        p.Add("to_utc", toUtc.Value, DbType.DateTime);
    }

    sql += """
    
    GROUP BY t.status
    ORDER BY count DESC, status ASC;
    """;

    using var conn = db.CreateConnection();

    var rows = await conn.QueryAsync<TicketsByStatusDto>(
        new CommandDefinition(sql, p, cancellationToken: ct)
    );

    return rows.AsList();
}

    public async Task<IReadOnlyList<TopClientDto>> GetTopClientsAsync(  int limit, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
    {
        const string sql ="""
        SELECT 
        c.id AS ClientId,
        c.name AS ClientName,
        COUNT(t.id)::int AS TicketsCount
        FROM clients c
        JOIN projects p ON p.client_id = c.id
        JOIN tickets t ON t.project_id = p.id
        GROUP BY c.id, c.name
        ORDER BY TicketsCount DESC , c.id ASC
        LIMIT @Limit;
        """;
        using var conn = db.CreateConnection();
        var rows = await conn.QueryAsync<TopClientDto>(
            new CommandDefinition(sql, new { Limit = limit }, cancellationToken: ct)
        );
        return rows.AsList();
    }
}
    