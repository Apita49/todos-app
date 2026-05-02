namespace Todos.Middlewares;

using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todos.Middlewares.Exceptions;

public class ErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
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
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ErrorHandler> logger)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        object response;

        switch (exception)
        {
            case TodoNotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                logger.LogInformation("Todo not found: {Message}", notFoundEx.Message);
                response = new { message = notFoundEx.Message };
                break;

            case TodoValidationException validationEx:
                statusCode = StatusCodes.Status400BadRequest;
                logger.LogWarning("Validation error: {@Errors}", validationEx.Errors);
                response = new { message = validationEx.Message, errors = validationEx.Errors };
                break;

            case SqliteException dbEx:
                statusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(dbEx, "Database error occurred");
                response = new { message = "A database error occurred" };
                break;

            case Exception ex:
                statusCode = StatusCodes.Status500InternalServerError;
                logger.LogError(ex, "Unhandled exception occurred");
                response = new { message = "An unexpected error occurred" };
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                response = new { message = "An unexpected error occurred" };
                break;
        }

        context.Response.StatusCode = statusCode;
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, jsonOptions);
        return context.Response.WriteAsync(json);
    }
}
