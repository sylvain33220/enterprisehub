using MediatR;

namespace EnterpriseHub.Application.Auth.Commands.Revoke;

public class RevokeCommand : IRequest
{
    public required string RefreshToken { get; set; }
    
}