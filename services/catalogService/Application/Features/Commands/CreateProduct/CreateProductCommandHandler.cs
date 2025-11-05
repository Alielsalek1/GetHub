using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductCommandRepository productCommandRepository, IProductQueryRepository productQueryRepository, ICategoryQueryRepository categoryQueryRepository) 
    : IRequestHandler<CreateProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validate category exists
        var categoryExists = await categoryQueryRepository.GetByIdAsync(request.CategoryId);
        if (categoryExists == null)
        {
            return Result.Fail(new CategoryNotFoundError());
        }

        // Check if product with same name already exists
        var existingProduct = await productQueryRepository.GetByNameAsync(request.Name);
        if (existingProduct != null)
        {
            return Result.Fail(new ProductAlreadyExistsError());
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            CategoryId = request.CategoryId
        };

    var savedProduct = await productCommandRepository.AddAsync(product);

        return Result.Ok(savedProduct.Id);
    }
}
