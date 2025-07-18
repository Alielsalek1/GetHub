using FluentResults;
using catalogService.Application.DTOs;
using catalogService.Domain.Entities;
using catalogService.Domain.Interfaces;
using SharedKernel.ResultPattern;

namespace catalogService.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductAttributeRepository _attributeRepository;

    public ProductService(IProductRepository productRepository, IProductAttributeRepository attributeRepository)
    {
        _productRepository = productRepository;
        _attributeRepository = attributeRepository;
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Handle attributes
        foreach (var attr in request.Attributes)
        {
            var attribute = await _attributeRepository.GetByNameAsync(attr.Key);
            if (attribute == null)
            {
                // Create attribute if it doesn't exist
                attribute = await _attributeRepository.CreateAsync(new ProductAttribute
                {
                    Name = attr.Key,
                    DataType = "string"
                });
            }

            product.AttributeValues.Add(new ProductAttributeValue
            {
                ProductId = product.Id,
                AttributeId = attribute.Id,
                Value = attr.Value
            });
        }

        var createdProduct = await _productRepository.CreateAsync(product);
        return Result.Ok(MapToResponse(createdProduct));
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result.Fail(new NotFoundError($"Product with ID {id} not found"));

        return Result.Ok(MapToResponse(product));
    }

    public async Task<Result<IEnumerable<ProductResponse>>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return Result.Ok(products.Select(MapToResponse));
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result.Fail(new NotFoundError($"Product with ID {id} not found"));

        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.Stock.HasValue) product.Stock = request.Stock.Value;
        if (request.ImageUrl != null) product.ImageUrl = request.ImageUrl;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
        product.UpdatedAt = DateTime.UtcNow;

        // Handle attribute updates
        if (request.Attributes != null)
        {
            product.AttributeValues.Clear();
            foreach (var attr in request.Attributes)
            {
                var attribute = await _attributeRepository.GetByNameAsync(attr.Key);
                if (attribute == null)
                {
                    attribute = await _attributeRepository.CreateAsync(new ProductAttribute
                    {
                        Name = attr.Key,
                        DataType = "string"
                    });
                }

                product.AttributeValues.Add(new ProductAttributeValue
                {
                    ProductId = product.Id,
                    AttributeId = attribute.Id,
                    Value = attr.Value
                });
            }
        }

        var updatedProduct = await _productRepository.UpdateAsync(product);
        return Result.Ok(MapToResponse(updatedProduct));
    }

    public async Task<Result> DeleteProductAsync(Guid id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
            return Result.Fail(new NotFoundError($"Product with ID {id} not found"));

        await _productRepository.DeleteAsync(id);
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<ProductResponse>>> SearchProductsByAttributeAsync(string attributeName, string value)
    {
        var products = await _productRepository.SearchByAttributeAsync(attributeName, value);
        return Result.Ok(products.Select(MapToResponse));
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Attributes = product.AttributeValues.ToDictionary(av => av.Attribute.Name, av => av.Value)
        };
    }
}
