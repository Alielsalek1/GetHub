using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.CookiePolicy;
using authService.filters;
using FluentValidation.AspNetCore;
using FluentValidation;
using authService.Interfaces;
using authService.Services;
using AutoMapper;
using authService.Utils;
using MassTransit;

namespace authService;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // public void AddCustomAuth(IServiceCollection services)
    // {
    //     services.AddAuthentication(options =>
    //         {
    //             options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //             options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //             options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //         })
    //         .AddCookie()
    //         .AddJwtBearer(options =>
    //         {
    //             options.SaveToken = true;
    //             options.TokenValidationParameters = new TokenValidationParameters
    //             {
    //                 ValidateIssuer = true,
    //                 ValidateAudience = true,
    //                 ValidateIssuerSigningKey = true,
    //                 ValidateLifetime = true,
    //                 ValidIssuer = Configuration["JwtSettings:Issuer"],
    //                 ValidAudience = Configuration["JwtSettings:Audience"],
    //                 IssuerSigningKey = new SymmetricSecurityKey(
    //                     Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]!))
    //             };
    //         }).AddGoogle(options =>
    //         {
    //             options.ClientId = Configuration["Google:ClientId"]!;
    //             options.ClientSecret = Configuration["Google:ClientSecret"]!;
    //         });
    // }

    public void AddCookiePolicyOptions(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = HttpOnlyPolicy.Always;
        });
    }
    // public void AddControllersAndFilters(IServiceCollection services)
    // {
    //     services.AddControllers();
    //     services.AddScoped<ValidateInput>();
    // }

    public void AddServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IAuthService, AuthService>();

        // services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
    }

    public void AddRepositories(IServiceCollection services)
    {

    }

    public void AddMassTransit(IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("rabbitmq", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("auth", false));
            });
        });
    }
}