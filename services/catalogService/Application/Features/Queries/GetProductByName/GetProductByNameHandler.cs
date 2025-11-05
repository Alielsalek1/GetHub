using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Queries.GetProductByName;

public class GetProductByNameHandler(IProductQueryRepository productRepository) : IRequestHandler<GetProductByNameQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByNameAsync(request.Name);

        if (product == null)
            return Result.Fail(new ProductNotFoundError());

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
