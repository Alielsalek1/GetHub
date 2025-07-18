using SharedKernel.Extensions;
using SharedKernel.Middleware;

var builder = WebApplication.CreateBuilder(args);

// configure JWT authentication and authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var startup = new authService.Startup(builder.Configuration);

builder.Services.AddControllers();
startup.AddServices(builder.Services);

startup.AddMassTransit(builder.Services);
// startup.AddRepositories(builder.Services);

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