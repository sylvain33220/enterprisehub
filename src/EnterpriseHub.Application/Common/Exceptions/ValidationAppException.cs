namespace EnterpriseHub.Application.Common.Exceptions;

public sealed class ValidationAppException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(string message)
        : base(message, HttpStatus.BadRequest, "Validation failed", "validation_error")
    {
        Errors = new Dictionary<string, string[]>
        {
            ["general"] = new[] { message }
        };
    }

    public ValidationAppException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", HttpStatus.BadRequest, "Validation failed", "validation_error")
    {
        Errors = errors;
    }
}