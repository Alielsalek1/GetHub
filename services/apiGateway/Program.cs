using Yarp.ReverseProxy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateway.Middlewares;
using SharedKernel.Services;
using Serilog;

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

    // Routing to services from the Reverse Proxy Configuration
    builder.Services
        .AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    var app = builder.Build();

    // Custom middleware to manage JWT tokens before routing to services via YARP
    app.UseJwtTransformation(builder.Configuration);

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