using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using transactionService.Application.Queries.Interfaces;
using transactionService.Domain.Interfaces;
using transactionService.Infrastructure.Persistence;
using transactionService.Infrastructure.Repositories;

namespace transactionService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContexts
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<TransactionDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<TransactionReadDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add Repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ITransactionQueryRepository, TransactionQueryRepository>();

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
