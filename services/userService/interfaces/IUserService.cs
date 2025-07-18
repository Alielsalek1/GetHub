using FluentResults;
using userService.DTOs;

namespace userService.interfaces;
public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest dto);
    Task<Result<UserResponse>> GetUserByIdAsync(Guid userId);
    Task<Result<UserResponse>> UpdateUserAsync(Guid userId, UpdateUserRequest dto);
    // Task<Result> DeleteUserAsync(Guid userId);
}