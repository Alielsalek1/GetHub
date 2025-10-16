using Microsoft.AspNetCore.Builder;
using Shared.Middleware;

namespace Shared.Extensions;

/// <summary>
/// Extension methods for configuring exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the application pipeline
    /// using this while using the Result pattern only for safety
    /// </summary>
    /// <param name="app">The application builder to add middleware to</param>
    /// <returns>The application builder for method chaining</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
