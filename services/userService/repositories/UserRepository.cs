using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using userService.interfaces;
using userService.Models;

namespace userService.repositories;

/// <summary>
/// Repository implementation for managing user data persistence in MongoDB.
/// Provides CRUD operations for user entities with async support.
/// </summary>
public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<User> _collection = database.GetCollection<User>("users");

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if
    /// found, otherwise null.</returns>
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _collection.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the user was created successfully.</returns>
    public async Task<bool> CreateUserAsync(User user)
    {
        await _collection.InsertOneAsync(user);
        return true;
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the user was updated successfully.</returns>
    public async Task<bool> UpdateUserAsync(User user)
    {
        var result = await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if
    /// found, otherwise null.</returns>
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

}