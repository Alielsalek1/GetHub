using SharedKernel.Extensions;
using SharedKernel.Middleware;
using Serilog;
using Serilog.Debugging;

// Enable Serilog internal debugging
SelfLog.Enable(Console.Error);

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
    Log.Information("Starting AuthService");

    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and add Serilog
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

// configure JWT authentication and authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var startup = new authService.Startup(builder.Configuration);

startup.AddHttpClients(builder.Services);
builder.Services.AddControllers();
startup.AddServices(builder.Services);
startup.AddMongoDB(builder.Services);
startup.AddRepositories(builder.Services);

startup.AddMassTransit(builder.Services);

// startup.AddCustomAuth(builder.Services);

// startup.AddCookiePolicyOptions(builder.Services);

var app = builder.Build();

// global error handler from SharedKernel
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}