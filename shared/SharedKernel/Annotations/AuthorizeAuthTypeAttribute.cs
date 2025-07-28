using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Enums;

namespace SharedKernel.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeAuthTypeAttribute(params AuthType[] authTypes) : Attribute, IAuthorizationFilter
{
    private readonly AuthType[] _requiredAuthTypes = authTypes ?? throw new ArgumentNullException(nameof(authTypes));

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (_requiredAuthTypes.Contains(AuthType.Anonymous))
        {
            return;
        }

        // Check if user is authenticated (handled by ASP.NET Core auth middleware)
        if (!context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get the auth_type claim from the JWT token
        var authTypeClaim = context.HttpContext.User.FindFirst("auth_type")?.Value;
        
        if (string.IsNullOrEmpty(authTypeClaim))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Determine the actual auth type from the token
        var actualAuthType = authTypeClaim.ToLowerInvariant() switch
        {
            "service" => AuthType.Service,
            "user" => AuthType.User,
            "anonymous" => AuthType.Anonymous,
            _ => AuthType.User // Default to user for unknown types
        };

        // Check if the actual auth type is allowed
        var isAllowed = _requiredAuthTypes.Contains(actualAuthType) ||
                       (_requiredAuthTypes.Contains(AuthType.UserOrService) && 
                        (actualAuthType == AuthType.User || actualAuthType == AuthType.Service));

        if (!isAllowed)
        {
            context.Result = new ForbidResult();
        }
    }
}
