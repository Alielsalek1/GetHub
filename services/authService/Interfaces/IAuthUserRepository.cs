using authService.models;
using FluentResults;

namespace authService.Interfaces;

public interface IAuthUserRepository
{
    Task CreateAsync(AuthUser user);
    Task<AuthUser> GetByIdAsync(Guid id);
    Task<AuthUser> GetByEmailAsync(string email);
    Task UpdateAsync(AuthUser user);
}
