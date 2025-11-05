using System.Collections.Concurrent;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;

namespace CatalogService.Tests.Infrastructure;

public class InMemoryCategoryCommandRepository : ICategoryCommandRepository
{
    private int _nextId = 1;
    private readonly ConcurrentDictionary<int, Category> _store;

    public InMemoryCategoryCommandRepository(ConcurrentDictionary<int, Category> store)
    {
        _store = store;
    }

    public Task<Category> AddAsync(Category category)
    {
        category.Id = _nextId++;
        _store[category.Id] = category;
        return Task.FromResult(category);
    }

    public Task UpdateAsync(Category category)
    {
        _store[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category)
    {
        _store.TryRemove(category.Id, out _);
        return Task.CompletedTask;
    }
}
