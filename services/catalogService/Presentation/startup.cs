using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.Extensions.DependencyInjection; // Ensure IServiceCollection is available
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CatalogService.Presentation;

/// <summary>
/// Startup class responsible for configuring services and application components
/// for the User Service microservice.
/// </summary>
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Configures Swagger/OpenAPI documentation services for the application.
    /// Enables API documentation and testing interface in development environment.
    /// </summary>
    /// <param name="services">The service collection to add Swagger services to</param>
    public void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    /// <summary>
    /// Registers application-specific services and their implementations.
    /// Configures dependency injection for user service and repository layers.
    /// </summary>
    /// <param name="services">The service collection to register application services to</param>
    public void ConfigureServices(IServiceCollection services)
    {
        
    }

    /// <summary>
    /// Configures Serilog structured logging for the application.
    /// Clears default logging providers and sets up Serilog with configuration from appsettings.json.
    /// Enables centralized logging to Seq and console outputs.
    /// </summary>
    /// <param name="builder">The web application builder to configure logging for</param>
    public void ConfigureSerilog(WebApplicationBuilder builder)
    {
        // Clear default logging providers and add Serilog
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

}
