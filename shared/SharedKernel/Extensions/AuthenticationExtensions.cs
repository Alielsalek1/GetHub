using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SharedKernel.Extensions;
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
        .AddJwtBearer("External", options =>
        {

        })
        .AddJwtBearer("Internal", options =>
        {

        });

        services.AddAuthorization();
        return services;
    }
}
