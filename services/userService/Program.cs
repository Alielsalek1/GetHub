using MongoDB.Driver;
using userService.Models;
using userService;
using userService.Endpoints;
using SharedKernel.Middleware;
using Microsoft.AspNetCore.Builder;
using SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);
startup.CofigureMongoDB(builder.Services);
startup.ConfigureSwagger(builder.Services);
builder.Services.AddControllers();

var app = builder.Build();

// global exception handler
app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await new Startup(builder.Configuration).InitializeMongoAsync(app);
app.UseHttpsRedirection();
app.MapUsersEndpoints();

app.Run();
