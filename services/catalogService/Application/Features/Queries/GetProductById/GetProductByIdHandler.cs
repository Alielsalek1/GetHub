using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Queries.GetProductById;

public class GetProductByIdHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);

        if (product == null)
            return Result.Fail(new NotFoundError($"Product with ID {request.Id} not found"));

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };

        return Result.Ok(response);
    }
}
