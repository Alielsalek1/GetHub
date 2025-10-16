using Microsoft.AspNetCore.Http;
using Shared.DTOs;

namespace Shared.Utils;

/// <summary>
/// Utility class for extracting user information from HTTP headers
/// set by the API Gateway authentication middleware
/// </summary>
public static class HeaderExtractor
{
    /// <summary>
    /// Header names used by the API Gateway middleware
    /// </summary>
    private static class HeaderNames
    {
        public const string UserId = "X-User-ID";
        public const string UserName = "X-User-Name";
        public const string UserEmail = "X-User-Email";
        public const string UserRoles = "X-User-Roles";
        public const string AuthType = "X-Auth-Type";
    }

    /// <summary>
    /// Extracts user information from HTTP request headers
    /// </summary>
    /// <param name="headers">The request headers containing user information</param>
    /// <returns>UserHeadersDto containing extracted user information</returns>
    public static UserHeadersDto ExtractUserHeaders(IHeaderDictionary? headers)
    {
        if (headers == null)
            return new UserHeadersDto();

        return new UserHeadersDto
        {
            UserId = GetHeaderValue(headers, HeaderNames.UserId),
            UserName = GetHeaderValue(headers, HeaderNames.UserName),
            UserEmail = GetHeaderValue(headers, HeaderNames.UserEmail),
            UserRoles = GetHeaderValue(headers, HeaderNames.UserRoles),
            AuthType = GetHeaderValue(headers, HeaderNames.AuthType)
        };
    }

    /// <summary>
    /// Gets the User ID from X-User-ID header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>User ID or null if not found</returns>
    public static string? GetUserId(IHeaderDictionary? headers)
    {
        return headers != null ? GetHeaderValue(headers, HeaderNames.UserId) : null;
    }

    /// <summary>
    /// Gets the Username from X-User-Name header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>Username or null if not found</returns>
    public static string? GetUserName(IHeaderDictionary? headers)
    {
        return headers != null ? GetHeaderValue(headers, HeaderNames.UserName) : null;
    }

    /// <summary>
    /// Gets the User Email from X-User-Email header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>User email or null if not found</returns>
    public static string? GetUserEmail(IHeaderDictionary? headers)
    {
        return headers != null ? GetHeaderValue(headers, HeaderNames.UserEmail) : null;
    }

    /// <summary>
    /// Gets the Auth Type from X-Auth-Type header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>Auth type (admin, customer, anonymous) or null if not found</returns>
    public static string? GetAuthType(IHeaderDictionary? headers)
    {
        return headers != null ? GetHeaderValue(headers, HeaderNames.AuthType) : null;
    }

    /// <summary>
    /// Gets the User Roles from X-User-Roles header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>Comma-separated roles string or null if not found</returns>
    public static string? GetUserRoles(IHeaderDictionary? headers)
    {
        return headers != null ? GetHeaderValue(headers, HeaderNames.UserRoles) : null;
    }

    /// <summary>
    /// Gets the User Roles as an array from X-User-Roles header
    /// </summary>
    /// <param name="headers">The request headers</param>
    /// <returns>Array of roles or empty array if not found</returns>
    public static string[] GetUserRolesArray(IHeaderDictionary? headers)
    {
        var rolesString = GetUserRoles(headers);
        return rolesString?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(r => r.Trim())
                          .ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// Safely gets a header value as string
    /// </summary>
    /// <param name="headers">Header dictionary</param>
    /// <param name="headerName">Name of the header to retrieve</param>
    /// <returns>Header value or null if not found</returns>
    private static string? GetHeaderValue(IHeaderDictionary headers, string headerName)
    {
        if (headers.TryGetValue(headerName, out var values))
            return values.FirstOrDefault();
        return null;
    }
}