using authService.models;

namespace authService.Interfaces;

public interface IUserRepository
{
    Task<AuthUser> GetByIdAsync(Guid id);
    Task AddAsync(AuthUser user);
    Task UpdateAsync(AuthUser user);
}