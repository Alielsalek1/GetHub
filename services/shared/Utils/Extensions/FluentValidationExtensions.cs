using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Shared.DTOs;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Shared.Extensions;

/// <summary>
/// Extension methods for configuring FluentValidation in ASP.NET Core applications
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Configures FluentValidation with custom API response format for validation errors
    /// </summary>
    /// <param name="services">The service collection to configure validation for</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection UseFluentValidationWithApiResponse(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToArray();

                var result = Result.Fail(new ValidationError());

                return (IActionResult) result.ToErrorApiResult(
                    additionalErrors: [.. errors]
                );
            };
        });

        return services;
    }
}