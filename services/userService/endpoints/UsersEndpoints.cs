using FluentResults;
using SharedKernel;
using SharedKernel.Extensions;
using userService.DTOs;
using userService.interfaces;
using userService.Models;

namespace userService.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (CreateUserRequest request, IUserService userService) =>
        {
            var result = await userService.CreateUserAsync(request);
            return result
                .ToApiResult(
                    successMessage: "User created successfully",
                    successStatusCode: 200
                );
        });

        app.MapGet("/users/me", async (Guid id, IUserService userService) =>
        {
            var result = await userService.GetUserByIdAsync(id);
            return result.ToApiResult(
                successMessage: "User retrieved successfully",
                successStatusCode: 200
            );
        });

        app.MapPut("/users/me", async (Guid id, UpdateUserRequest updatedDto, IUserService userService) =>
        {
            var result = await userService.UpdateUserAsync(id, updatedDto);
            return result.ToApiResult(
                successMessage: "User updated successfully",
                successStatusCode: 200
            );
        });

        // app.MapDelete("/users/{id}", async (Guid id, IUserService userService) =>
        // {
        //     var result = await userService.DeleteUserAsync(id);
        //     return result.ToApiResult(
        //         successStatusCode: 204
        //     );
        // });
    }
}