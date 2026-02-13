namespace EnterpriseHub.Application.Dashboard.Dtos;

public sealed record TopClientDto(
    int ClientId,
    string ClientName,
    int TicketCount
);