namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base(message, HttpStatus.Unauthorized, "Unauthorized", "unauthorized")
    {
    }
}