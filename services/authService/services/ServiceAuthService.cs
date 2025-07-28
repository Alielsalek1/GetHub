using FluentResults;
using authService.Interfaces;
using authService.Dtos;
using System.Security.Claims;
using SharedKernel;
using SharedKernel.Services;

namespace authService.Services;

public class ServiceAuthService(IConfiguration configuration) : IServiceAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public Task<Result<string>> IssueServiceTokenAsync()
    {
        // Validate service secret from header
        var expectedSecret = _configuration["ServiceToService:Secret"] ?? throw new ArgumentNullException("ServiceToService:Secret", "Service secret configuration is missing");
    
        // Get internal JWT configuration
        var internalSecret = _configuration["Jwt:Internal:Secret"] ?? throw new ArgumentNullException("Jwt:Internal:Secret", "Internal JWT secret configuration is missing");
        var internalIssuer = _configuration["Jwt:Internal:Issuer"] ?? throw new ArgumentNullException("Jwt:Internal:Issuer", "Internal JWT issuer configuration is missing");
        var internalAudience = _configuration["Jwt:Internal:Audience"] ?? throw new ArgumentNullException("Jwt:Internal:Audience", "Internal JWT audience configuration is missing");
    
        try
        {
            // Create claims for service-to-service token
            var claims = new List<Claim>
            {
                new("auth_type", "service"),
            };

            var tokenString = JwtTokenService.CreateJwtToken(claims, internalSecret, internalIssuer, internalAudience);
            return Task.FromResult(Result.Ok(tokenString));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail<string>(new ValidationError($"Failed to generate service token: {ex.Message}")));
        }
    }

}
