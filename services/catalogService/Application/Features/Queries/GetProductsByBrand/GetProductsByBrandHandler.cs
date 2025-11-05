using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Queries.GetProductsByBrand;

public class GetProductsByBrandHandler(IProductQueryRepository productRepository) : IRequestHandler<GetProductsByBrandQuery, Result<List<ProductResponse>>>
{
    public async Task<Result<List<ProductResponse>>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllProductsByBrandAsync(request.Brand);

        var response = products.Select(product => new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        }).ToList();

        return Result.Ok(response);
    }
}
