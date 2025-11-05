using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductQueryRepository productQueryRepository, IProductCommandRepository productCommandRepository, ICategoryQueryRepository categoryQueryRepository) 
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productQueryRepository.GetByIdAsync(request.Id);
        
        if (product == null)
            return Result.Fail(new ProductNotFoundError());

        // Validate category exists if changing category
        if (request.CategoryId.HasValue && request.CategoryId != product.CategoryId)
        {
            var categoryExists = await categoryQueryRepository.GetByIdAsync(request.CategoryId.Value);
            if (categoryExists == null)
                return Result.Fail(new CategoryNotFoundError());
        }

        // Check for name conflicts if changing name
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != product.Name)
        {
            var existingProduct = await productQueryRepository.GetByNameAsync(request.Name);
            if (existingProduct != null && existingProduct.Id != request.Id)
                return Result.Fail(new ProductAlreadyExistsError());
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

    await productCommandRepository.UpdateAsync(updatedProduct);

        return Result.Ok();
    }
}
