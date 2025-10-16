using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Extensions;
using Shared.Enums;
using Shared.Annotations;
using userService.DTOs;
using userService.interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Serilog;
using MassTransit;
using Shared.Utils;
using FluentResults;

namespace userService.Controllers;

/// <summary>
/// Controller responsible for managing user operations including creation, retrieval, and updates.
/// Provides endpoints for user management with proper authorization and logging.
/// </summary>
[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly Serilog.ILogger logger = Log.ForContext<UsersController>();

    /// <summary>
    /// Creates a new user with the provided details.
    /// This endpoint is restricted to service-to-service calls only.
    /// </summary>
    /// <param name="request">DTO containing user details for creation</param>
    /// <returns>A result with created user data including the generated user ID</returns>
    /// <response code="200">User created successfully</response>
    /// <response code="400">Invalid input data or user creation failed</response>
    /// <response code="403">Unauthorized access - service authentication required</response>
    [HttpPost("{UserId:guid}")]
    [AuthorizeAuthType(AuthType.Customer, AuthType.Admin)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, Guid UserId)
    {
        logger.Information("Creating user with Id: {Id}", UserId);
        
        var result = await userService.CreateUserAsync(request, UserId);

        if (result.IsSuccess)
        {
            logger.Information("User created successfully with Id: {Id}", UserId);
            return result.ToSuccessApiResult(
                successStatusCode: 201
            );
        }

        logger.Warning("User creation failed for Id: {Id}. Errors: {Errors}",
                UserId, string.Join(", ", result.Errors.Select(e => e.Message)));
        return result.ToErrorApiResult();
    }

    /// <summary>
    /// Retrieves the current user's information based on the JWT token.
    /// Extracts user ID from the token claims and returns user details.
    /// </summary>
    /// <returns>A result containing the current authenticated user's information</returns>
    /// <response code="200">User information retrieved successfully</response>
    /// <response code="400">Invalid user ID in token</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized - valid authentication required</response>
    [HttpGet("me")]
    [AuthorizeAuthType(AuthType.Customer, AuthType.Admin)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = HeaderExtractor.GetUserId(Request.Headers);
        if (userId == null || !Guid.TryParse(userId, out var userIdGuid))
            return Result.Fail(new ValidationError()).ToErrorApiResult(["Invalid Request"]);

        var result = await userService.GetUserByIdAsync(userIdGuid);

        if (result.IsSuccess)
        {
            logger.Information("Current user retrieved successfully: {UserId}", userId);
            return result.ToSuccessApiResult(
                successStatusCode: 200,
                successMessage: "User retrieved successfully"
            );
        }
        logger.Warning("Failed to retrieve current user: {UserId}. Errors: {Errors}",
            userId, string.Join(", ", result.Errors.Select(e => e.Message)));
        return result.ToErrorApiResult();
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// This endpoint is restricted to service-to-service calls only for security purposes.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the user to retrieve</param>
    /// <returns>A result containing the requested user's information</returns>
    /// <response code="200">User information retrieved successfully</response>
    /// <response code="404">User not found with the specified ID</response>
    /// <response code="403">Unauthorized access - service authentication required</response>
    /// <response code="400">Invalid user ID format</response>
    [HttpGet("{userId:guid}")]
    [AuthorizeAuthType(AuthType.Admin)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        logger.Information("Getting user by ID: {UserId}", userId);

        var result = await userService.GetUserByIdAsync(userId);

        if (result.IsSuccess)
        {
            logger.Information("User retrieved successfully: {UserId}", userId);
            return result.ToSuccessApiResult(
                successStatusCode: 200,
                successMessage: "User retrieved successfully"
            );
        }
        logger.Warning("Failed to retrieve user: {UserId}. Errors: {Errors}",
            userId, string.Join(", ", result.Errors.Select(e => e.Message)));
        return result.ToErrorApiResult();
    }

    /// <summary>
    /// Updates the current authenticated user's information.
    /// Extracts the user ID from JWT token claims and applies the provided updates.
    /// </summary>
    /// <param name="updateUserDto">DTO containing the updated user details</param>
    /// <returns>A result indicating the success or failure of the update operation</returns>
    /// <response code="200">User information updated successfully</response>
    /// <response code="400">Invalid user ID in token or invalid update data</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized - user authentication required</response>
    [HttpPut("me")]
    [AuthorizeAuthType(AuthType.Customer, AuthType.Admin)]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserRequest updateUserDto)
    {
        var userId = HeaderExtractor.GetUserId(Request.Headers);
        if (userId == null || !Guid.TryParse(userId, out var userIdGuid))
            return Result.Fail(new ValidationError()).ToErrorApiResult(["Invalid Request"]);

        var result = await userService.UpdateUserAsync(userIdGuid, updateUserDto);

        if (result.IsSuccess)
        {
            logger.Information("User updated successfully: {UserId}", userId);
            return result.ToSuccessApiResult(
                successStatusCode: 200,
                successMessage: "User updated successfully"
            );
        }
        logger.Warning("Failed to update user: {UserId}. Errors: {Errors}",
            userId, string.Join(", ", result.Errors.Select(e => e.Message)));
        return result.ToErrorApiResult();
    }
}