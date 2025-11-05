using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using userService.DTOs;
using userService.repositories;
using Xunit;

namespace GetHub.Tests.UserService;

/// <summary>
/// API-level integration tests that boot the userService app in-memory and exercise controller endpoints.
/// These tests forge the headers added by the API Gateway middleware (X-Auth-Type, X-User-ID) to validate
/// authorization behavior without running the gateway.
/// </summary>
public class ControllerIntegrationTests
{
    private static HttpClient CreateClient()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services
                        .AddControllers()
                        .AddApplicationPart(typeof(userService.Controllers.UsersController).Assembly);
                    services.AddAuthorization();
                    services.AddSingleton<userService.interfaces.IUserRepository, InMemoryUserRepository>();
                    services.AddScoped<userService.interfaces.IUserService, userService.services.UserService>();
                });
                webHost.Configure(app =>
                {
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            });

        var host = hostBuilder.Start();
        return host.GetTestClient();
    }

    [Fact]
    public async Task Post_users_UserId_CreatesUser_WhenCustomer()
    {
    using var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Auth-Type", "customer");
        var userId = Guid.NewGuid();
        client.DefaultRequestHeaders.Add("X-User-ID", userId.ToString());

        var payload = new CreateUserRequest { phoneNumber = "+19998887777", address = "Nowhere" };
        var res = await client.PostAsJsonAsync($"/api/user/{userId}", payload);
        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Get_me_Returns401_WhenMissingHeaders()
    {
    using var client = CreateClient();
        // No headers -> should be unauthorized by [AuthorizeAuthType]
        var res = await client.GetAsync("/api/user/me");
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_me_Returns200_WhenCustomerWithValidUserId()
    {
    using var client = CreateClient();
        var userId = Guid.NewGuid();
        client.DefaultRequestHeaders.Add("X-Auth-Type", "customer");
        client.DefaultRequestHeaders.Add("X-User-ID", userId.ToString());

        // create first
        var payload = new CreateUserRequest { phoneNumber = "+12223334444", address = "Somewhere" };
        var create = await client.PostAsJsonAsync($"/api/user/{userId}", payload);
        create.EnsureSuccessStatusCode();

        var res = await client.GetAsync("/api/user/me");
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
