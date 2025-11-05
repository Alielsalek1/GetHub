using CatalogService.Domain.models;

namespace CatalogService.Application.Interfaces;

public interface ICategoryCommandRepository
{
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}
