using CatalogService.Domain.models;

namespace CatalogService.Application.Interfaces;

public interface IProductCommandRepository
{
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
