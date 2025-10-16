using userService.DTOs;
using userService.interfaces;
using userService.Models;
using FluentResults;
using Shared;

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
    /// <param name="Dto">The user creation request containing user details</param>
    /// <returns>A result containing the created user information or error details</returns>
    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest Dto, Guid UserId)
    {
        var user = new User
        {
            Id = UserId,
            PhoneNumber = Dto.phoneNumber,
            Address = Dto.address,
        };
        
        if (await userRepository.GetUserByIdAsync(user.Id) != null)
            return Result.Fail(new UserAlreadyExistsError());

        await userRepository.CreateUserAsync(user);

        return Result.Ok();
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to retrieve</param>
    /// <returns>A result containing the user information if found, or error details if not found</returns>
    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid UserId)
    {
        var user = await userRepository.GetUserByIdAsync(UserId);

        if (user == null)
            return Result.Fail(new UserNotFoundError());

        return Result.Ok(new UserResponse
        {
            id = user.Id,
            phoneNumber = user.PhoneNumber,
            address = user.Address,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl,
            bankAccountNumber = user.BankAccountNumber
        });
    }

    /// <summary>
    /// Updates an existing user's information with the provided details.
    /// Only updates fields that are provided in the request (partial update).
    /// </summary>
    /// <param name="UserId">The unique identifier of the user to update</param>
    /// <param name="Dto">The update request containing the fields to modify</param>
    /// <returns>A result containing the updated user information or error details</returns>
    public async Task<Result<UserResponse>> UpdateUserAsync(Guid UserId, UpdateUserRequest Dto)
    {
        var user = await userRepository.GetUserByIdAsync(UserId);

        if (user == null)
            return Result.Fail(new UserNotFoundError());

        var newUser = new User
        {
            Id = user.Id,
            Bio = Dto.bio ?? user.Bio,
            ProfileImageUrl = Dto.profileImageUrl ?? user.ProfileImageUrl,
            PhoneNumber = Dto.phoneNumber ?? user.PhoneNumber,
            Address = Dto.address ?? user.Address,
            BankAccountNumber = Dto.bankAccountNumber ?? user.BankAccountNumber
        };

        await userRepository.UpdateUserAsync(newUser);

        return Result.Ok();
    }

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete</param>
    /// <returns>A result indicating the success or failure of the delete operation</returns>
    public async Task<Result> DeleteUserAsync(Guid userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
            return Result.Fail(new UserNotFoundError());

        await userRepository.DeleteUserAsync(userId);

        return Result.Ok();
    }
}