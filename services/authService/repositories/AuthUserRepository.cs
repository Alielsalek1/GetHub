using authService.Interfaces;
using authService.models;
using FluentResults;
using MongoDB.Driver;
using SharedKernel;

namespace authService.repositories;

/// <summary>
/// Repository implementation for managing authentication user data in MongoDB.
/// Handles CRUD operations for authentication-specific user entities.
/// </summary>
public class AuthUserRepository(IMongoDatabase database) : IAuthUserRepository
{
    private readonly IMongoCollection<AuthUser> authUsers = database.GetCollection<AuthUser>("authUsers");

    /// <summary>
    /// Creates a new authentication user in the database.
    /// </summary>
    /// <param name="user">The authentication user entity to create</param>
    /// <returns>A task representing the asynchronous create operation</returns>
    public async Task CreateAsync(AuthUser user)
    {
        await authUsers.InsertOneAsync(user);
    }

    /// <summary>
    /// Retrieves an authentication user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve</param>
    /// <returns>A task containing the authentication user if found, otherwise null</returns>
    public async Task<AuthUser> GetByIdAsync(Guid id)
    {
        var user = await authUsers.Find(u => u.Id == id).FirstOrDefaultAsync();
        return user;
    }

    /// <summary>
    /// Retrieves an authentication user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve</param>
    /// <returns>A task containing the authentication user if found, otherwise null</returns>
    public async Task<AuthUser> GetByEmailAsync(string email)
    {
        var user = await authUsers.Find(u => u.Email == email).FirstOrDefaultAsync();
        return user;
    }

    /// <summary>
    /// Updates an existing authentication user in the database.
    /// </summary>
    /// <param name="user">The authentication user entity with updated information</param>
    /// <returns>A task representing the asynchronous update operation</returns>
    public async Task UpdateAsync(AuthUser user)
    {
        var result = await authUsers.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}