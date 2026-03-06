using System.Diagnostics;
using EnterpriseHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseHub.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
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
        catch (ValidationAppException ex)
        {
            _logger.LogWarning(ex, "Handled validation exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, ex.StatusCode, ex.Title, ex.Message, ex.ErrorCode, ex.Errors);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled app exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblem(context, ex.StatusCode, ex.Title, ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            var status = HttpStatus.InternalServerError;
            var title = "Internal Server Error";
            var detail = _env.IsDevelopment()
                ? ex.Message
                : "An unexpected error occurred.";

            await WriteProblem(context, status, title, detail, "server_error");
        }
    }

    private static async Task WriteProblem(
        HttpContext ctx,
        int status,
        string title,
        string detail,
        string? errorCode = null,
        object? errors = null)
    {
        if (ctx.Response.HasStarted)
        {
            return;
        }
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";

        var traceId = Activity.Current?.Id ?? ctx.TraceIdentifier;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}",
            Instance = ctx.Request.Path
        };

        problem.Extensions["traceId"] = traceId;
        problem.Extensions["method"] = ctx.Request.Method;
        problem.Extensions["path"] = ctx.Request.Path;
    
        if (!string.IsNullOrWhiteSpace(errorCode))
            problem.Extensions["code"] = errorCode;

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        await ctx.Response.WriteAsJsonAsync(problem);
    }
}