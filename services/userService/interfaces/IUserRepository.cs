using userService.Models;

namespace userService.interfaces;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid userId);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid userId);
}