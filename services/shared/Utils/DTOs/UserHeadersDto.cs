namespace Shared.DTOs;

/// <summary>
/// Data Transfer Object containing user information extracted from HTTP headers
/// set by the API Gateway authentication middleware
/// </summary>
public class UserHeadersDto
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRoles { get; set; }
    public string? AuthType { get; set; }
    public string[] Roles => UserRoles?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(r => r.Trim())
                              .ToArray() ?? Array.Empty<string>();
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(AuthType);
    public bool IsAdmin => string.Equals(AuthType, "admin", StringComparison.OrdinalIgnoreCase);
    public bool IsCustomer => string.Equals(AuthType, "customer", StringComparison.OrdinalIgnoreCase);
}