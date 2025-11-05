using System.Collections.Concurrent;
using userService.interfaces;
using userService.Models;

namespace userService.repositories;

/// <summary>
/// In-memory implementation of <see cref="IUserRepository"/> intended for testing and local development.
/// This repository stores users in a concurrent dictionary and is NOT meant for production use.
/// </summary>
/// <remarks>
/// Usage: consume via DI in tests or sample hosts to avoid external dependencies like MongoDB.
/// Thread-safe for simple test scenarios; no persistence across runs.
/// </remarks>
public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _store = new();

    public Task<User> GetUserByIdAsync(Guid userId)
    {
        _store.TryGetValue(userId, out var user);
        return Task.FromResult(user!);
    }

    public Task<bool> CreateUserAsync(User user)
    {
        var created = _store.TryAdd(user.Id, user);
        return Task.FromResult(created);
    }

    public Task<bool> UpdateUserAsync(User user)
    {
        _store[user.Id] = user;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteUserAsync(Guid userId)
    {
        var removed = _store.TryRemove(userId, out _);
        return Task.FromResult(removed);
    }
}
