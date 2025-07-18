using FluentResults;
using catalogService.Application.DTOs;

namespace catalogService.Application.Services;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request);
    Task<Result<ProductResponse>> GetProductByIdAsync(Guid id);
    Task<Result<IEnumerable<ProductResponse>>> GetAllProductsAsync();
    Task<Result<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<Result> DeleteProductAsync(Guid id);
    Task<Result<IEnumerable<ProductResponse>>> SearchProductsByAttributeAsync(string attributeName, string value);
}
