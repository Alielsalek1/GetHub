using catalogService.Domain.Entities;

namespace catalogService.Domain.Interfaces;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByIdAsync(int id);
    Task<ProductAttribute?> GetByNameAsync(string name);
    Task<IEnumerable<ProductAttribute>> GetAllAsync();
    Task<ProductAttribute> CreateAsync(ProductAttribute attribute);
    Task<ProductAttribute> UpdateAsync(ProductAttribute attribute);
    Task<bool> DeleteAsync(int id);
}
