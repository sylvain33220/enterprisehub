namespace EnterpriseHub.Application.Dashboard.Dtos;

public sealed class TicketsByStatusDto
{
    public string Status { get; init; } = default!;
    public int Count { get; init; }
}