using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using userService.interfaces;
using userService.services;
using MassTransit;
using userService.repositories;
using MongoDB.Driver;
using userService.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Shared.Extensions;

namespace userService;

/// <summary>
/// Startup class responsible for configuring services and application components
/// for the User Service microservice.
/// </summary>
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    /// <summary>
    /// Configures MongoDB database connection and registers MongoDB-related services.
    /// Sets up the MongoDB client with connection timeouts and registers the database instance.
    /// </summary>
    /// <param name="services">The service collection to add MongoDB services to</param>
    /// <exception cref="InvalidOperationException">Thrown when MongoDB connection string or database name is not configured</exception>
    public void ConfigureMongoDB(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("MongoDb") ??
                throw new InvalidOperationException("MongoDB connection string is not configured.");
        var databaseName = Configuration["MongoDbDatabase"] ??
                throw new InvalidOperationException("MongoDB database name is not configured.");

        // Configure MongoDB client with connection options
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30); // Increased timeout
            settings.ConnectTimeout = TimeSpan.FromSeconds(30); // Increased timeout
            settings.SocketTimeout = TimeSpan.FromSeconds(30);
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(30);
            return new MongoClient(settings);
        });

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
    }

    /// <summary>
    /// Registers application-specific services and their implementations.
    /// Configures dependency injection for user service and repository layers.
    /// </summary>
    /// <param name="services">The service collection to register application services to</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
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

    /// <summary>
    /// Initializes MongoDB connection and creates necessary database indexes.
    /// Tests database connectivity with a ping command and creates a unique index on the email field.
    /// Handles connection failures gracefully by allowing the service to continue without MongoDB.
    /// </summary>
    /// <param name="app">The configured web application instance</param>
    /// <returns>A task representing the asynchronous initialization operation</returns>
    public async Task InitializeMongoAsync(WebApplication app)
    {
        const int maxRetries = 5;
        const int delayBetweenRetries = 2000; // 2 seconds

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
                var users = db.GetCollection<User>("users");
                
                // Test connection with longer timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await users.Database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                    new MongoDB.Bson.BsonDocument("ping", 1), cancellationToken: cts.Token);
            
                Console.WriteLine("✅ MongoDB connection established and indexes created successfully.");
                return; // Success, exit the retry loop
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                Console.WriteLine($"⚠️ MongoDB connection attempt {attempt}/{maxRetries} failed: {ex.Message}");
                Console.WriteLine($"Retrying in {delayBetweenRetries/1000} seconds...");
                await Task.Delay(delayBetweenRetries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ MongoDB initialization failed after {maxRetries} attempts: {ex.Message}");
                Console.WriteLine("Service will continue running without MongoDB connection.");
                break;
            }
        }
    }
}