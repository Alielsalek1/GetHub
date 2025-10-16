using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Extensions;
using FluentResults;

namespace Shared.Annotations;

/// <summary>
/// Authorization attribute that restricts access based on authentication type from JWT claims
/// </summary>
/// <param name="authTypes">Array of allowed authentication types for the endpoint</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeAuthTypeAttribute(params AuthType[] authTypes) : Attribute, IAuthorizationFilter
{
    private readonly AuthType[] _requiredAuthTypes = authTypes ?? throw new ArgumentNullException(nameof(authTypes));

    /// <summary>
    /// Performs authorization logic based on the X-Auth-Type header set by the API Gateway middleware
    /// </summary>
    /// <param name="context">The authorization filter context containing request information</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (_requiredAuthTypes.Contains(AuthType.Anonymous))
            return;

        // Get the auth type from the X-Auth-Type header set by the API Gateway middleware
        var authTypeHeader = context.HttpContext.Request.Headers["X-Auth-Type"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authTypeHeader))
        {
            var unauthorizedResult = Result.Fail(new UnauthorizedError());
            context.Result = unauthorizedResult.ToErrorApiResult();
            return;
        }

        // Determine the actual auth type from the header
        var actualAuthType = authTypeHeader.ToLowerInvariant() switch
        {
            "admin" => AuthType.Admin,
            "customer" => AuthType.Customer,
            _ => AuthType.Anonymous // Default to Anonymous for unknown types
        };

        // Check if the actual auth type is allowed
        var isAllowed = _requiredAuthTypes.Contains(actualAuthType);

        if (!isAllowed)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizeAuthTypeAttribute>>();
            logger?.LogWarning("Authorization failed. Required: [{RequiredTypes}], Actual: {ActualType}", 
                string.Join(", ", _requiredAuthTypes), actualAuthType);
            
            var forbiddenResult = Result.Fail(new ForbiddenError());
            context.Result = forbiddenResult.ToErrorApiResult();
        }
    }
}
