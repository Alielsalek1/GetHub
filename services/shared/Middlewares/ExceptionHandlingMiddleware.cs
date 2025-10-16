using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Enums;

namespace Shared.Middleware;

/// <summary>
/// Middleware for handling unhandled exceptions globally across the application
/// </summary>
/// <param name="next">The next middleware delegate in the pipeline</param>
/// <param name="logger">Logger instance for logging exceptions</param>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the middleware to handle HTTP requests and catch any unhandled exceptions
    /// </summary>
    /// <param name="context">The HTTP context for the current request</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, ex.Message);

            var apiResp = new ApiResponse
            {
                message = "An unexpected error occurred.",
                errorCode = ErrorCodes.UNEXPECTED_ERROR,
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(apiResp);
        }
    }
}
