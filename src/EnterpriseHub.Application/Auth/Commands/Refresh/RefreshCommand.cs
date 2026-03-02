using MediatR;
using EnterpriseHub.Application.Auth.Dto;

namespace EnterpriseHub.Application.Auth.Commands.Refresh;
public class RefreshCommand : IRequest<AuthTokens>
{
    public required string RefreshToken { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}