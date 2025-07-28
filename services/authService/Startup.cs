using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.CookiePolicy;
using authService.filters;
using FluentValidation.AspNetCore;
using FluentValidation;
using authService.Interfaces;
using authService.Services;
using AutoMapper;
using authService.Utils;
using MassTransit;
using System.Net.Http.Headers;
using MongoDB.Driver;
using authService.repositories;

namespace authService;

/// <summary>
/// Startup class responsible for configuring services and components for the Authentication Service.
/// Provides methods to configure authentication, authorization, database connections, and external services.
/// </summary>
public class Startup(IConfiguration configuration)
{
    /// <summary>
    /// Configures custom authentication services including Google OAuth.
    /// Currently commented out but provides framework for cookie and Google authentication.
    /// </summary>
    /// <param name="services">The service collection to add authentication services to</param>
    public void AddCustomAuth(IServiceCollection services)
    {
        // services.AddAuthentication(options =>
        //     {
        //         options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //     })
        //     .AddCookie()
        //     .AddGoogle(options =>
        //     {
        //         options.ClientId = Configuration["Google:ClientId"]!;
        //         options.ClientSecret = Configuration["Google:ClientSecret"]!;
        //     });
    }

    /// <summary>
    /// Configures cookie policy options for secure cookie handling.
    /// Sets up minimum same-site policy, secure policy, and HTTP-only policy for enhanced security.
    /// </summary>
    /// <param name="services">The service collection to add cookie policy configuration to</param>
    public void AddCookiePolicyOptions(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = HttpOnlyPolicy.Always;
        });
    }

    // public void AddControllersAndFilters(IServiceCollection services)
    // {
    //     services.AddControllers();
    //     services.AddScoped<ValidateInput>();
    // }

    /// <summary>
    /// Registers application services including authentication services, AutoMapper, and Swagger.
    /// Configures dependency injection for core authentication and service auth functionality.
    /// </summary>
    /// <param name="services">The service collection to register application services to</param>
    public void AddServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IServiceAuthService, ServiceAuthService>();

        // services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
    }

    /// <summary>
    /// Registers repository implementations for data access layer.
    /// Configures dependency injection for authentication user repository.
    /// </summary>
    /// <param name="services">The service collection to register repositories to</param>
    public void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
    }

    /// <summary>
    /// Configures MongoDB database connection and services.
    /// Sets up MongoDB client with connection timeouts and registers database instance.
    /// </summary>
    /// <param name="services">The service collection to add MongoDB services to</param>
    /// <exception cref="ArgumentException">Thrown when MongoDB configuration is missing</exception>
    public void AddMongoDB(IServiceCollection services)
    {
        var connectionString = configuration["MONGODB_CONNECTION_STRING"] ?? 
                            throw new ArgumentException("MONGODB_CONNECTION_STRING is not set in configuration");
        var databaseName = configuration["MONGODB_DATABASE_NAME"] ??
                            throw new ArgumentException("MONGODB_DATABASE_NAME is not set in configuration");

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(5);
            return new MongoClient(settings);
        });

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });
    }

    /// <summary>
    /// Configures MassTransit message bus with RabbitMQ for inter-service communication.
    /// Sets up RabbitMQ connection with default credentials and endpoint naming conventions.
    /// </summary>
    /// <param name="services">The service collection to add MassTransit services to</param>
    public void AddMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("auth", false));
            });
        });
    }

    /// <summary>
    /// Configures HTTP clients for external service communication.
    /// Sets up HTTP client for communicating with the user service with proper headers.
    /// </summary>
    /// <param name="services">The service collection to add HTTP client services to</param>
    public void AddHttpClients(IServiceCollection services)
    {
        services.AddHttpClient("users", client =>
        {
            client.BaseAddress = new Uri("http://user-service:5082/");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }
}