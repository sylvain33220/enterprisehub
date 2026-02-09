
using EnterpriseHub.Domain.Enums;
namespace EnterpriseHub.Application.Tickets.Dto;


public record TicketDto(
    Guid Id,
    string Title,
    string? Description,
    TicketPriority Priority,
    TicketStatus Status,
    Guid ProjectId
);
