using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using userService.interfaces;
using userService.Models;

namespace userService.repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");
    } 

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _collection.Find(u => u.Id == userId).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        await _collection.InsertOneAsync(user);
        return true;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var result = await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
        return result.ModifiedCount > 0;
    }

    public Task<User> GetUserByEmailAsync(string email)
    {
        return _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    // public async Task<bool> DeleteUserAsync(Guid userId)
    // {
    //     var result = await _collection.DeleteOneAsync(u => u.Id == userId);
    //     return result.DeletedCount > 0;
    // }
}