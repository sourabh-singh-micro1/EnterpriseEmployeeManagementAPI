using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseEmployeeManagementAPI.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var statusCode = exception switch
            {
                InvalidOperationException => StatusCodes.Status409Conflict,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            logger.LogError(
                exception,
                "Request {Method} {Path} failed with status code {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                statusCode);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeNames.Application.ProblemJson;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = context.Request.Path
            };
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            await context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
        }
    }
}
