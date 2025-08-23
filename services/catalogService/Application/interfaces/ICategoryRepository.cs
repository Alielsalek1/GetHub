using CatalogService.Domain.models;

namespace CatalogService.Application.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetByNameAsync(string name);
    Task<List<Category>> GetFirstLevelCategoriesAsync(int id);
    Task<List<Category>> GetCategoryWithAncestorsAsync(int id);
    Task<List<Category>> GetBaseCategoriesAsync();
    Task UpdateAsync(Category category);
}