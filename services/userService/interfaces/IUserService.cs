using FluentResults;
using userService.DTOs;

namespace userService.interfaces;
public interface IUserService
{
    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest Dto, Guid UserId);
    Task<Result<UserResponse>> GetUserByIdAsync(Guid UserId);
    Task<Result<UserResponse>> UpdateUserAsync(Guid UserId, UpdateUserRequest Dto);
    Task<Result> DeleteUserAsync(Guid userId);
}