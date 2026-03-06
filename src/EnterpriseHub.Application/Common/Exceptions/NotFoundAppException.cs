namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string message)
     : base(message, HttpStatus.NotFound, "Not Found", "not_found") { }
}