using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository) 
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        
        if (product == null)
            return Result.Fail(new NotFoundError($"Product with ID {request.Id} not found"));

        // Validate category exists if changing category
        if (request.CategoryId.HasValue && request.CategoryId != product.CategoryId)
        {
            var categoryExists = await categoryRepository.GetByIdAsync(request.CategoryId.Value);
            if (categoryExists == null)
                return Result.Fail(new NotFoundError($"Category with ID {request.CategoryId} not found"));
        }

        // Check for name conflicts if changing name
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != product.Name)
        {
            var existingProduct = await productRepository.GetByNameAsync(request.Name);
            if (existingProduct != null && existingProduct.Id != request.Id)
                return Result.Fail(new AlreadyExistsError($"Product with name '{request.Name}' already exists"));
        }

        // Create updated product entity
        var updatedProduct = new Product
        {
            Id = request.Id,
            Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : product.Name,
            Description = request.Description ?? product.Description,
            Brand = request.Brand ?? product.Brand,
            CategoryId = request.CategoryId ?? product.CategoryId
        };

        await productRepository.UpdateAsync(updatedProduct);

        return Result.Ok();
    }
}
