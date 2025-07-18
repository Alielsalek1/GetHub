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

namespace userService;
public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void CofigureMongoDB(IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(Configuration.GetConnectionString("MongoDb")));

        services.AddScoped(sp =>
            sp.GetRequiredService<IMongoClient>()
            .GetDatabase(Configuration["MongoDbDatabase"]));
    }

    public void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public void configureJwt(IServiceCollection services)
    {
        
    }

    public async Task InitializeMongoAsync(WebApplication app)
    {
        var db = app.Services.GetRequiredService<IMongoDatabase>();
        var users = db.GetCollection<User>("users");
        await users.Indexes.CreateOneAsync(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));
    }
}
