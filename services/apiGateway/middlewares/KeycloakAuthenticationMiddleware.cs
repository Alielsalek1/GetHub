using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiGateway.Middlewares;

public class KeycloakAuthenticationMiddleware(
    RequestDelegate next,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<KeycloakAuthenticationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract token from Authorization header
        // if token doesn't exists, skip authentication
        var token = ExtractTokenFromHeader(context.Request);
        if (string.IsNullOrEmpty(token))
        {
            logger.LogInformation("No Authorization header found, skipping authentication");
            logger.LogInformation("Forwarding request without user information");
            await next(context);
            return;
        }

        // convert the user info from the token(if exists) to headers
        await AddUserInfoAsync(context, token);

        logger.LogInformation("User authenticated successfully");
        logger.LogInformation("Forwarding user information to downstream services");
        // Continue to next middleware
        await next(context);
    }

    private async Task AddUserInfoAsync(HttpContext context, string token)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient("KeycloakClient");
            var realm = configuration["Keycloak:Realm"] ?? throw new InvalidOperationException("Keycloak:Realm not configured");
            var clientId = configuration["Keycloak:Confidential:ClientId"] ?? throw new InvalidOperationException("Keycloak:Confidential:ClientId not configured");
            var clientSecret = configuration["Keycloak:Confidential:ClientSecret"] ?? throw new InvalidOperationException("Keycloak:Confidential:ClientSecret not configured");

            var introspectUrl = $"/realms/{realm}/protocol/openid-connect/token/introspect";

            logger.LogInformation("Introspecting token with Keycloak at {IntrospectUrl}", introspectUrl);
            logger.LogInformation("Client ID: {ClientId}", clientId);
            logger.LogInformation("Client Secret: {ClientSecret}", clientSecret); // Avoid logging sensitive info
            logger.LogInformation("Token: {Token}", token);
            logger.LogInformation("Realm: {Realm}", realm);

            var formData = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            ]);

            // Log the exact request details
            logger.LogInformation("========== REQUEST DETAILS ==========");
            logger.LogInformation("Full URL: {Url}", $"{httpClient.BaseAddress}{introspectUrl}");
            logger.LogInformation("Content-Type: {ContentType}", formData.Headers.ContentType?.ToString());
            var formContent = await formData.ReadAsStringAsync();
            logger.LogInformation("Form Data: {FormData}", formContent);
            logger.LogInformation("Token length: {TokenLength} characters", token.Length);
            logger.LogInformation("=====================================");

            // Recreate formData since we consumed it for logging
            formData = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("token", token),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            ]);

            var response = await httpClient.PostAsync(introspectUrl, formData);

            // Add user info to headers
            var content = await response.Content.ReadAsStringAsync();
            logger.LogInformation("Keycloak introspection response: {Response}", content);
            var tokenInfo = JsonSerializer.Deserialize<TokenIntrospectionResponse>(content);
            if (tokenInfo == null)
            {
                logger.LogWarning("Failed to deserialize token introspection response");
                return;
            }
            logger.LogInformation("transferring user data");
            logger.LogInformation("User ID: {UserId}", tokenInfo.Sub);
            logger.LogInformation("Username: {Username}", tokenInfo.Username);
            logger.LogInformation("Email: {Email}", tokenInfo.Email);
            logger.LogInformation("Roles: {Roles}", tokenInfo.RealmAccess?.Roles != null ? string.Join(",", tokenInfo.RealmAccess.Roles) : "None");
            logger.LogInformation("Sub: {Sub}", tokenInfo.Sub);
            logger.LogInformation("active: {Active}", tokenInfo.Active);
            if (IsValidToken(tokenInfo) == false)
            {
                logger.LogWarning("Token is not active");
                return;
            }

            // Add user claims as headers for downstream services - only extract what we need
            if (!string.IsNullOrEmpty(tokenInfo.Sub))
                context.Request.Headers["X-User-ID"] = tokenInfo.Sub;
            if (!string.IsNullOrEmpty(tokenInfo.Username))
                context.Request.Headers["X-User-Name"] = tokenInfo.Username;
            if (!string.IsNullOrEmpty(tokenInfo.Email))
                context.Request.Headers["X-User-Email"] = tokenInfo.Email;
            if (tokenInfo.RealmAccess?.Roles != null && tokenInfo.RealmAccess.Roles.Any())
                context.Request.Headers["X-User-Roles"] = string.Join(",", tokenInfo.RealmAccess.Roles);
            
            // Determine auth type based on roles
            var authType = DetermineAuthType(tokenInfo.RealmAccess?.Roles);
            context.Request.Headers["X-Auth-Type"] = authType;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating token with Keycloak");
        }
    }

    private static string? ExtractTokenFromHeader(HttpRequest request)
    {
        var authHeader = request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return authHeader["Bearer ".Length..].Trim();
    }

    private static bool IsValidToken(TokenIntrospectionResponse token)
    {
        return token?.Active == true;
    }

    private static string DetermineAuthType(string[]? roles)
    {
        if (roles == null || !roles.Any())
            return "anonymous";

        // Check for admin role first (higher priority)
        if (roles.Contains("admin", StringComparer.OrdinalIgnoreCase))
            return "admin";

        // Check for customer role
        if (roles.Contains("customer", StringComparer.OrdinalIgnoreCase))
            return "customer";

        // Default to user if no specific role found
        return "anonymous";
    }
}

// Token introspection response models
public class TokenIntrospectionResponse
{
    [JsonPropertyName("active")]
    public bool Active { get; set; }
    
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }
    
    [JsonPropertyName("preferred_username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }
    
    [JsonPropertyName("exp")]
    public long Exp { get; set; }
    
    [JsonPropertyName("iat")]
    public long Iat { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
    
    [JsonPropertyName("realm_access")]
    public RealmAccess? RealmAccess { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }
    
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }
    
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
}

public class RealmAccess
{
    [JsonPropertyName("roles")]
    public string[]? Roles { get; set; }
}