using Microsoft.AspNetCore.Builder;
using SharedKernel.Middleware;

namespace SharedKernel.Extensions;
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
