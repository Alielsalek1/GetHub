using orderService.Application.Extensions;
using orderService.Infrastructure.Extensions;
using orderService.Endpoints;
using SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Application layer (MediatR, FluentValidation, etc.)
builder.Services.AddApplication();

// Add Infrastructure layer (DbContext, Repositories, MassTransit)
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add exception handling middleware
app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapOrderEndpoints();

app.Run();
