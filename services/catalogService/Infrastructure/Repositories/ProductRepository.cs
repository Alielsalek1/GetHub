using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories;

public class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public async Task<Product> AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return product;
    }

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

    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Products.AnyAsync(p => p.Id == id);
    }
}
