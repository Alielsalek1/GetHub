using Microsoft.Extensions.DependencyInjection;

namespace Shared.Extensions;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation.
/// 
/// Usage Instructions:
/// 1. Install Swashbuckle.AspNetCore NuGet package in your microservice
/// 2. Call services.ConfigureSwagger() in your Startup/Program.cs
/// 3. Call app.UseSwaggerInDevelopment() in your middleware pipeline
/// 
/// Example:
/// services.ConfigureSwagger();
/// app.UseSwaggerInDevelopment();
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configures Swagger/OpenAPI documentation services for the application.
    /// Enables API documentation and testing interface in development environment.
    /// 
    /// Prerequisites: Install Swashbuckle.AspNetCore NuGet package in your microservice project.
    /// </summary>
    /// <param name="services">The service collection to add Swagger services to</param>
    /// <param name="configureSwagger">Optional action to configure SwaggerGen options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, Action<object>? configureSwagger = null)
    {
        // Add endpoints API explorer
        services.AddEndpointsApiExplorer();
        
        // Note: AddSwaggerGen() call should be made in the consuming microservice
        // since it requires Swashbuckle.AspNetCore dependency
        // 
        // In your microservice Startup/Program.cs, add:
        // services.ConfigureSwagger();
        // services.AddSwaggerGen(); // <-- Add this line
        
        return services;
    }
}