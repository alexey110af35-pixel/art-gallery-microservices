using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Shared.Middleware;

public class GlobalExceptionHandler
{
	private readonly RequestDelegate _next;
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
			_logger.LogError(ex, "An unhandled exception occurred");
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";

		var (statusCode, message) = exception switch
		{
			UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
			ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
			KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
			InvalidOperationException => (StatusCodes.Status500InternalServerError, exception.Message), // ← Показываем реальное сообщение
			_ => (StatusCodes.Status500InternalServerError, "An internal server error occurred.")
		};

		context.Response.StatusCode = statusCode;

		var response = new
		{
			status = statusCode,
			message,
			detail = exception.Message,
			timestamp = DateTime.UtcNow
		};

		return context.Response.WriteAsync(JsonSerializer.Serialize(response));
	}
}

// Extension method для удобного добавления в pipeline
public static class ExceptionHandlerExtensions
{
	public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
	{
		return app.UseMiddleware<GlobalExceptionHandler>();
	}
}