using System.Diagnostics;
using EnterpriseHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace EnterpriseHub.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled app exception");
            await WriteProblem(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var (status, title) = ex switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Validation error"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
                _ => (StatusCodes.Status500InternalServerError, "Server error")
            };

            // ✅ ne pas leak en prod sur 500
            var detail = status == 500 && !_env.IsDevelopment()
                ? "An unexpected error occurred."
                : ex.Message;

            await WriteProblem(context, status, detail, title);
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, string detail, string? title = null)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";

        var traceId = Activity.Current?.Id ?? ctx.TraceIdentifier;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title ?? ReasonPhrases.GetReasonPhrase(status),
            Detail = detail,
            Instance = ctx.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        await ctx.Response.WriteAsJsonAsync(problem);
    }
}