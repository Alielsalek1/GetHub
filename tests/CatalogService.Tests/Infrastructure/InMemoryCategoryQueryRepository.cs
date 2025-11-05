using System.Collections.Concurrent;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;

namespace CatalogService.Tests.Infrastructure;

public class InMemoryCategoryQueryRepository : ICategoryQueryRepository
{
    private readonly ConcurrentDictionary<int, Category> _store;

    public InMemoryCategoryQueryRepository(ConcurrentDictionary<int, Category> store)
    {
        _store = store;
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        _store.TryGetValue(id, out var category);
        return Task.FromResult(category);
    }

    public Task<Category?> GetByNameAsync(string name)
    {
        var category = _store.Values.FirstOrDefault(c => c.Name == name);
        return Task.FromResult(category);
    }

    public Task<List<Category>> GetBaseCategoriesAsync()
    {
        var categories = _store.Values.Where(c => c.ParentId == null).ToList();
        return Task.FromResult(categories);
    }

    public Task<List<Category>> GetFirstLevelCategoriesAsync(int parentId)
    {
        var categories = _store.Values.Where(c => c.ParentId == parentId).ToList();
        return Task.FromResult(categories);
    }

    public Task<List<Category>> GetCategoryWithAncestorsAsync(int categoryId)
    {
        var result = new List<Category>();
        
        if (!_store.TryGetValue(categoryId, out var category))
            return Task.FromResult(result);

        result.Add(category);

        var currentParentId = category.ParentId;
        while (currentParentId.HasValue && _store.TryGetValue(currentParentId.Value, out var parent))
        {
            result.Add(parent);
            currentParentId = parent.ParentId;
        }

        return Task.FromResult(result);
    }

    public Task<bool> HasChildCategoriesAsync(int id)
    {
        var hasChildren = _store.Values.Any(c => c.ParentId == id);
        return Task.FromResult(hasChildren);
    }
}
