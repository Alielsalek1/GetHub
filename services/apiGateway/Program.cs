using Yarp.ReverseProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;
using ApiGateway.Middlewares;

// Configure Serilog from appSettings.Json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting ApiGateway service");

    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and add Serilog
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    // Add dedicated HttpClient for Keycloak communication to prevent socket exhaustion
    builder.Services.AddHttpClient("KeycloakClient", client =>
    {
        var keycloakUrl = builder.Configuration["Keycloak:Url"] ?? throw new InvalidOperationException("Keycloak:Url not configured");
        client.BaseAddress = new Uri(keycloakUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
    {
        // Configure connection pooling to prevent socket exhaustion
        MaxConnectionsPerServer = 10,
        UseDefaultCredentials = false
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Rotate handlers every 5 minutes

    // Routing to services from the Reverse Proxy Configuration
    builder.Services
        .AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    // Add Keycloak authentication middleware
    app.UseMiddleware<KeycloakAuthenticationMiddleware>();

    // using YARP for Reverse Proxy Routing
    app.MapReverseProxy();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ApiGateway service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}