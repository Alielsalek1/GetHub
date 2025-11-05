using FluentResults;
using userService.DTOs;

namespace userService.interfaces;
public interface IUserService
{
    Task<Result> CreateUserAsync(CreateUserRequest Dto, Guid UserId);
    Task<Result<UserResponse>> GetUserByIdAsync(Guid UserId);
    Task<Result> UpdateUserAsync(Guid UserId, UpdateUserRequest Dto);
    Task<Result> DeleteUserAsync(Guid userId);
}