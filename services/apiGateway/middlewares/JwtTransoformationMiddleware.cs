using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Services;
using Serilog;

namespace ApiGateway.Middlewares;
public static class JwtTransformationMiddleware
{
    /// <summary>
    /// Middleware to transform JWT tokens from external services to internal format.
    /// </summary>
    /// <param name="app">The application builder to use for configuring the middleware.</param>
    /// <param name="configuration">The configuration containing JWT settings.</param>
    /// <returns>A Jwt token for the internal service.</returns>
    public static IApplicationBuilder UseJwtTransformation(this IApplicationBuilder app, IConfiguration configuration)
    {
        return app.Use(async (context, next) =>
        {
            var logger = Log.ForContext("Middleware", "JwtTransformation");
            var requestPath = context.Request.Path.Value;
            
            logger.Information("Processing request {RequestPath} with JWT transformation", requestPath);

            // Read configuration for JWT settings
            var externalIssuer = configuration["Jwt:External:Issuer"] ?? throw new ArgumentNullException("Jwt:External:Issuer");
            var externalAudience = configuration["Jwt:External:Audience"] ?? throw new ArgumentNullException("Jwt:External:Audience");
            var externalSecret = configuration["Jwt:External:Secret"] ?? throw new ArgumentNullException("Jwt:External:Secret");
            var internalIssuer = configuration["Jwt:Internal:Issuer"] ?? throw new ArgumentNullException("Jwt:Internal:Issuer");
            var internalAudience = configuration["Jwt:Internal:Audience"] ?? throw new ArgumentNullException("Jwt:Internal:Audience");
            var internalSecret = configuration["Jwt:Internal:Secret"] ?? throw new ArgumentNullException("Jwt:Internal:Secret");

            // Read the Authorization header
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

            // Check if the Authorization header contains a Bearer token    
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                logger.Information("External JWT token found for request {RequestPath}", requestPath);
                
                // Extract external token
                var externalToken = authHeader["Bearer ".Length..].Trim();
                try
                {
                    // Validate external token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = externalIssuer,
                        ValidateAudience = true,
                        ValidAudiences = externalAudience?.Split(','),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(externalSecret)),
                        ValidateLifetime = true
                    };
                    var principal = tokenHandler.ValidateToken(externalToken, validationParams, out var validatedToken);

                    // Generating Required Claims for the Token
                    var claims = JwtTokenService.GetExtraClaims(principal.Claims).ToList();
                    claims.Add(new Claim("auth_type", "user"));

                    var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var userEmail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                    logger.Information("Successfully validated external token for user {UserId} {UserEmail} on request {RequestPath}", 
                        userId, userEmail, requestPath);

                    var internalToken = JwtTokenService.CreateJwtToken(claims, internalSecret, internalIssuer, internalAudience);
                    context.Request.Headers.Authorization = $"Bearer {internalToken}";
                    
                    logger.Information("Generated internal token for authenticated user on request {RequestPath}", requestPath);
                }
                catch (SecurityTokenValidationException ex)
                {
                    logger.Warning("JWT token validation failed for request {RequestPath}: {ErrorMessage}", requestPath, ex.Message);
                    
                    // if token validation fails, return Unauthorized
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("");
                    return;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Unexpected error during JWT token validation for request {RequestPath}", requestPath);
                    
                    // if token validation fails, return Unauthorized
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("");
                    return;
                }
            }
            else
            {
                logger.Information("No external token found, generating anonymous token for request {RequestPath}", requestPath);
                
                // If no token is present, create an anonymous token
                var claims = new List<Claim>
                {
                    new("auth_type", "anonymous")
                };

                var anonymousToken = JwtTokenService.CreateJwtToken(claims, internalSecret, internalIssuer, internalAudience);
                context.Request.Headers.Authorization = $"Bearer {anonymousToken}";
                
                logger.Information("Generated anonymous token for request {RequestPath}", requestPath);
            }

            await next();
        });
    }
}