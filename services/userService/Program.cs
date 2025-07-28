using MongoDB.Driver;
using userService.Models;
using userService;
using SharedKernel.Middleware;
using Microsoft.AspNetCore.Builder;
using SharedKernel.Extensions;
using FluentValidation;
using URLshortner.Dtos.Validators;
using Serilog;

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
    JwtAuthenticationExtensions.AddJwtAuthentication(builder.Services, builder.Configuration);
    builder.Services.AddAuthorization();

    startup.ConfigureServices(builder.Services);
    startup.ConfigureMongoDB(builder.Services);
    startup.ConfigureSwagger(builder.Services);

    builder.Services.UseFluentValidationWithApiResponse();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
    builder.Services.UseJsonValidator();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // input validation
    app.UseGlobalExceptionHandler();

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // application logic
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