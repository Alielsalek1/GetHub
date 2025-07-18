using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using orderService.Application.Queries.Interfaces;
using orderService.Domain.Interfaces;
using orderService.Infrastructure.Persistence;
using orderService.Infrastructure.Repositories;

namespace orderService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContexts
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<OrderReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();

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
