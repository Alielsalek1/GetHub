using Microsoft.EntityFrameworkCore;
using catalogService.Domain.Entities;
using catalogService.Domain.Interfaces;
using catalogService.Infrastructure.Persistence;

namespace catalogService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.AttributeValues)
            .ThenInclude(av => av.Attribute)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.AttributeValues)
            .ThenInclude(av => av.Attribute)
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByAttributeAsync(string attributeName, string value)
    {
        return await _context.Products
            .Include(p => p.AttributeValues)
            .ThenInclude(av => av.Attribute)
            .Where(p => p.IsActive && 
                       p.AttributeValues.Any(av => 
                           av.Attribute.Name == attributeName && 
                           av.Value.Contains(value)))
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
