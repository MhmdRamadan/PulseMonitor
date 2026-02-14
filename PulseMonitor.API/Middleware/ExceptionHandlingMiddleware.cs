using System.Net;
using System.Text.Json;
using PulseMonitor.Domain.Exceptions;
using FluentValidation;

namespace PulseMonitor.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleAsync(context, ex);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message, errors) = ex switch
        {
            NotFoundException n => (HttpStatusCode.NotFound, n.Message, (object?)null),
            DomainException d => (HttpStatusCode.BadRequest, d.Message, (object?)null),
            ValidationException v => (HttpStatusCode.BadRequest, "Validation failed", v.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList()),
            _ => (HttpStatusCode.InternalServerError, "An error occurred", (object?)null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        var body = new { message, errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}
