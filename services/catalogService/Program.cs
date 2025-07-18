using catalogService.Infrastructure.Persistence;
using catalogService.Domain.Interfaces;
using catalogService.Infrastructure.Repositories;
using catalogService.Application.Services;
using catalogService.Endpoints;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();

// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductAttributeService, ProductAttributeService>();

// JWT authentication (Keycloak)
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Internal", policy =>
        policy.RequireAuthenticatedUser()
              .AddAuthenticationSchemes("Internal"));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handler
app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCatalogEndpoints();

app.Run();
