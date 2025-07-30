using Microsoft.AspNetCore.Mvc;

namespace LinkForge.API.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IWebHostEnvironment environment)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, environment, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, IWebHostEnvironment environment, Exception ex)
    {
        logger.LogError(ex, "Unhandled exception {message}", ex.Message);
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
            Detail = string.Empty,
        };

        if (environment.IsDevelopment())
        {
            details.Detail = $"{context.Request.Method} {context.Request.Path}: {ex.Message}";
        }

        await context.Response.WriteAsJsonAsync(details);
    }
}