using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Extensions;
using SharedKernel.Enums;
using SharedKernel.Annotations;
using userService.DTOs;
using userService.interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Serilog;

namespace userService.Controllers;

/// <summary>
/// Controller responsible for managing user operations including creation, retrieval, and updates.
/// Provides endpoints for user management with proper authorization and logging.
/// </summary>
[ApiController]
[Route("users")]
[Authorize]
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
    [HttpPost]
    [AuthorizeAuthType(AuthType.Service)]
    public async Task<IResult> CreateUser([FromBody] CreateUserRequest request)
    {
        logger.Information("Creating user with email: {Email}", request.email);
        
        var result = await userService.CreateUserAsync(request);
        
        if (result.IsSuccess)
            logger.Information("User created successfully with email: {Email}", request.email);
        else
            logger.Warning("User creation failed for email: {Email}. Errors: {Errors}", 
                request.email, string.Join(", ", result.Errors.Select(e => e.Message)));
        
        return result
            .ToApiResult(
                successMessage: "User created successfully",
                successStatusCode: 200
            );
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
    [AuthorizeAuthType(AuthType.UserOrService)]
    public async Task<IResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        logger.Information("Getting current user with ID claim: {UserIdClaim}", userIdClaim);
        
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            logger.Warning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
            return Results.BadRequest(new ApiResponse("Invalid user ID", 400));
        }

        var result = await userService.GetUserByIdAsync(userId);
        
        if (result.IsSuccess)
            logger.Information("Current user retrieved successfully: {UserId}", userId);
        else
            logger.Warning("Failed to retrieve current user: {UserId}. Errors: {Errors}", 
                userId, string.Join(", ", result.Errors.Select(e => e.Message)));
        
        return result.ToApiResult(
            successMessage: "User retrieved successfully",
            successStatusCode: 200
        );
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
    [HttpGet("{id:guid}")]
    [AuthorizeAuthType(AuthType.Service)]
    public async Task<IResult> GetUserById(Guid id)
    {
        logger.Information("Getting user by ID: {UserId}", id);
        
        var result = await userService.GetUserByIdAsync(id);
        
        if (result.IsSuccess)
            logger.Information("User retrieved successfully: {UserId}", id);
        else
            logger.Warning("Failed to retrieve user: {UserId}. Errors: {Errors}", 
                id, string.Join(", ", result.Errors.Select(e => e.Message)));
        
        return result.ToApiResult(
            successMessage: "User retrieved successfully",
            successStatusCode: 200
        );
    }

    /// <summary>
    /// Updates the current authenticated user's information.
    /// Extracts the user ID from JWT token claims and applies the provided updates.
    /// </summary>
    /// <param name="updatedDto">DTO containing the updated user details</param>
    /// <returns>A result indicating the success or failure of the update operation</returns>
    /// <response code="200">User information updated successfully</response>
    /// <response code="400">Invalid user ID in token or invalid update data</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized - user authentication required</response>
    [HttpPut("me")]
    [AuthorizeAuthType(AuthType.User)]
    public async Task<IResult> UpdateCurrentUser([FromBody] UpdateUserRequest updatedDto)
    {
        // Extract user ID from JWT claims  
        var userIdClaim = User.FindFirst("user_id")?.Value;

        
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Results.BadRequest(new ApiResponse("Invalid user ID in token", 400));

        var result = await userService.UpdateUserAsync(userId, updatedDto);
        return result.ToApiResult(
            successMessage: "User updated successfully",
            successStatusCode: 200
        );
    }
}