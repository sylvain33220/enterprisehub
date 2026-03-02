namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message = "Conflict") : base(message, 409) { }
}