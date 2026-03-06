namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message = "Forbidden") : base(message, HttpStatus.Forbidden, "Forbidden", "forbidden") { }
}