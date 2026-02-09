using EnterpriseHub.Domain.Enums;
namespace EnterpriseHub.Application.Tickets.Dto;

public record CreateTicketRequest(
    string Title,
    Guid ProjectId,
    string? Description,
    TicketPriority Priority
);