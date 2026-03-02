using EnterpriseHub.Application.Auth.Dto;
using MediatR;

namespace EnterpriseHub.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthTokens>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public  string? Ip { get; set; }
    public string? UserAgent { get; set; }
}