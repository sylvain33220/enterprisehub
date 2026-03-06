namespace EnterpriseHub.Application.Common.Exceptions;

public abstract class AppException(string message, int statusCode, string? title = null, string? errorCode = null) : Exception(message)
{
  public int StatusCode { get; } = statusCode;
  public string Title { get; } = title ?? message;
  public string? ErrorCode { get; } = errorCode;
}
public static class HttpStatus
{
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int InternalServerError = 500;
}