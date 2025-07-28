using userService.DTOs;
using userService.interfaces;
using userService.Models;
using FluentResults;
using SharedKernel;
using Amazon.Runtime;

namespace userService.services;

/// <summary>
/// Service implementation for managing user business logic and operations.
/// Handles user creation, retrieval, and updates with validation and error handling.
/// </summary>
public class UserService(IUserRepository userRepository) : IUserService
{
    /// <summary>
    /// Creates a new user with the provided details after validation.
    /// </summary>
    /// <param name="dto">The user creation request containing user details</param>
    /// <returns>A result containing the created user information or error details</returns>
    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest dto)
    {
        // use correct DTO property names
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.username,
            Email = dto.email,
            Bio = string.Empty,
            ProfileImageUrl = null
        };

        if (await userRepository.GetUserByEmailAsync(user.Email) != null)
            return Result.Fail(new AlreadyExistsError("User with this email already exists."));
            
        if (await userRepository.GetUserByIdAsync(user.Id) != null)
            return Result.Fail(new AlreadyExistsError("User with this ID already exists."));

        await userRepository.CreateUserAsync(user);

        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Username,
            email = user.Email,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl
        });
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to retrieve</param>
    /// <returns>A result containing the user information if found, or error details if not found</returns>
    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return Result.Fail(new NotFoundError("User not found."));

        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Username,
            email = user.Email,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl
        });
    }

    /// <summary>
    /// Updates an existing user's information with the provided details.
    /// Only updates fields that are provided in the request (partial update).
    /// </summary>
    /// <param name="id">The unique identifier of the user to update</param>
    /// <param name="dto">The update request containing the fields to modify</param>
    /// <returns>A result containing the updated user information or error details</returns>
    public async Task<Result<UserResponse>> UpdateUserAsync(Guid id, UpdateUserRequest dto)
    {
        var user = await userRepository.GetUserByIdAsync(id);

        if (user == null)
            return Result.Fail(new NotFoundError("User not found."));

        if (dto.name != null) user.Username = dto.name;
        if (dto.bio != null) user.Bio = dto.bio;
        if (dto.profileImageUrl != null) user.ProfileImageUrl = dto.profileImageUrl;

        await userRepository.UpdateUserAsync(user);
        
        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Username,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl
        });
    }

    // public async Task<Result> DeleteUserAsync(Guid userId)
    // {
    //     var user = await _userRepository.GetUserByIdAsync(userId);

    //     await _userRepository.DeleteUserAsync(user.Id);

    //     return Result.Ok();
    // }
}