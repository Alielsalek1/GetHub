using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository) 
    : IRequestHandler<CreateProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validate category exists
        var categoryExists = await categoryRepository.GetByIdAsync(request.CategoryId);
        if (categoryExists == null)
        {
            return Result.Fail(new NotFoundError($"Category with ID {request.CategoryId} not found"));
        }

        // Check if product with same name already exists
        var existingProduct = await productRepository.GetByNameAsync(request.Name);
        if (existingProduct != null)
        {
            return Result.Fail(new AlreadyExistsError($"Product with name '{request.Name}' already exists"));
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            CategoryId = request.CategoryId
        };

        var savedProduct = await productRepository.AddAsync(product);

        return Result.Ok(savedProduct.Id);
    }
}
