using Microsoft.EntityFrameworkCore;
using catalogService.Domain.Entities;
using catalogService.Domain.Interfaces;
using catalogService.Infrastructure.Persistence;

namespace catalogService.Infrastructure.Repositories;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private readonly CatalogDbContext _context;

    public ProductAttributeRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<ProductAttribute?> GetByIdAsync(int id)
    {
        return await _context.ProductAttributes.FindAsync(id);
    }

    public async Task<ProductAttribute?> GetByNameAsync(string name)
    {
        return await _context.ProductAttributes
            .FirstOrDefaultAsync(a => a.Name == name);
    }

    public async Task<IEnumerable<ProductAttribute>> GetAllAsync()
    {
        return await _context.ProductAttributes.ToListAsync();
    }

    public async Task<ProductAttribute> CreateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Add(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task<ProductAttribute> UpdateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attribute = await _context.ProductAttributes.FindAsync(id);
        if (attribute == null) return false;

        _context.ProductAttributes.Remove(attribute);
        await _context.SaveChangesAsync();
        return true;
    }
}
