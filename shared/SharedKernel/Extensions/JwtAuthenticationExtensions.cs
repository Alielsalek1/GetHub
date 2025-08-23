using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedKernel.Extensions;

/// <summary>
/// Extension methods for configuring JWT authentication services
/// </summary>
public static class JwtAuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication to the service collection with configuration from appsettings
    /// </summary>
    /// <param name="services">The service collection to add JWT authentication to</param>
    /// <param name="configuration">The configuration containing JWT settings</param>
    /// <returns>The service collection for method chaining</returns>
    /// <exception cref="InvalidOperationException">Thrown when required JWT configuration is missing</exception>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var secret = configuration["Jwt:Internal:Secret"] ?? 
                      throw new InvalidOperationException("JWT secret is not configured.");
        var issuer = configuration["Jwt:Internal:Issuer"] ?? 
                      throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = configuration["Jwt:Internal:Audience"] ?? 
                       throw new InvalidOperationException("JWT audience is not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
            };
            
        });

        return services;
    }
}
