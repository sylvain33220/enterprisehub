using EnterpriseHub.Domain.Enums;
namespace EnterpriseHub.Application.Tickets.Dto;

public record UpdateTicketRequest(
    string Title,
    string? Description,
    TicketStatus Status,
    TicketPriority Priority
);