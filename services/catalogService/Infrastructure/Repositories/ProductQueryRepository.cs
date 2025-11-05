using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CatalogService.Infrastructure.Repositories;

public class ProductQueryRepository(CatalogDbContext context) : IProductQueryRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<List<Product>> GetAllProductsByBrandAsync(string brand)
    {
        return await context.Products
            .AsNoTracking()
            .Where(p => p.Brand == brand)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Products.AnyAsync(p => p.Id == id);
    }
}
