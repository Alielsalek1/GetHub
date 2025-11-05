using CatalogService.Domain.models;

namespace CatalogService.Application.Interfaces;

public interface IProductQueryRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetByNameAsync(string name);
    Task<List<Product>> GetAllProductsByBrandAsync(string brand);
    Task<bool> ExistsAsync(int id);
}
