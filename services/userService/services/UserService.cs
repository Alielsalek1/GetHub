using userService.DTOs;
using userService.interfaces;
using userService.Models;
using FluentResults;

namespace userService.services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest dto)
    {
        var user = new User
        {
            Id = dto.userId,
            Name = dto.name,
            Email = dto.email,
            Bio = string.Empty,
            ProfileImageUrl = null
        };

        await _userRepository.CreateUserAsync(user);

        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Name,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl
        });
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Name,
            bio = user.Bio,
            profileImageUrl = user.ProfileImageUrl
        });
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(Guid id, UpdateUserRequest dto)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (dto.name != null) user.Name = dto.name;
        if (dto.bio != null) user.Bio = dto.bio;
        if (dto.profileImageUrl != null) user.ProfileImageUrl = dto.profileImageUrl;

        await _userRepository.UpdateUserAsync(user);
        
        return Result.Ok(new UserResponse
        {
            id = user.Id,
            name = user.Name,
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