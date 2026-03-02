namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message = "Not Found") : base(message, 404) { }
}