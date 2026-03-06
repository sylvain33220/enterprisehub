namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base(message, HttpStatus.Conflict, "Conflict", "conflict")
    {
    }
}