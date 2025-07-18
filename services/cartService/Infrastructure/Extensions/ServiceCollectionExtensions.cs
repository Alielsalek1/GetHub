using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using cartService.Application.Queries.Interfaces;
using cartService.Domain.Interfaces;
using cartService.Infrastructure.Persistence;
using cartService.Infrastructure.Repositories;

namespace cartService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContexts
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<CartDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<CartReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add Repositories
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartQueryRepository, CartQueryRepository>();

        // Add MassTransit for event publishing
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMQ"));
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
