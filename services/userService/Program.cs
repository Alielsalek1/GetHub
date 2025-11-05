using MongoDB.Driver;
using userService.Models;
using userService;
using Microsoft.AspNetCore.Builder;
using FluentValidation;
using Serilog;
using Shared.Extensions;
using userService.Validators;

// Configure Serilog from configuration
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
    Log.Information("Starting UserService");

    var builder = WebApplication.CreateBuilder(args);
    var startup = new Startup(builder.Configuration);
    
    startup.ConfigureSerilog(builder);
    builder.Services.AddControllers();
    startup.ConfigureServices(builder.Services);
    startup.ConfigureMongoDB(builder.Services);
    builder.Services.ConfigureSwagger();
    builder.Services.UseFluentValidationWithApiResponse();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
    
    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseGlobalExceptionHandler();
    // app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();   // Executes authorization filters like your AuthorizeAuthTypeAttribute
    app.MapControllers();
    await new Startup(builder.Configuration).InitializeMongoAsync(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UserService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}