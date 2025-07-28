using Microsoft.AspNetCore.Builder;
using SharedKernel.Middleware;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;

namespace SharedKernel.Extensions;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

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

                var apiResponse = new ApiResponse("Validation failed", 400, errors);
                return new BadRequestObjectResult(apiResponse);
            };
        });

        return services;
    }
    
    public static IServiceCollection UseJsonValidator(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToArray();

                var apiResponse = new ApiResponse("Invalid request format", 400, errors);
                return new BadRequestObjectResult(apiResponse);
            };
        });

        return services;
    }
}
