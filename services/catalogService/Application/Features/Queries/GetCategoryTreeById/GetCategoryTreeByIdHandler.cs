using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Queries.GetCategoryTreeById;

public class GetCategoryTreeByIdHandler(ICategoryQueryRepository categoryRepository) 
    : IRequestHandler<GetCategoryTreeByIdQuery, Result<CategoryTreeResponse>>
{
    public async Task<Result<CategoryTreeResponse>> Handle(GetCategoryTreeByIdQuery request, CancellationToken cancellationToken)
    {
        var categoriesWithAncestors = await categoryRepository.GetCategoryWithAncestorsAsync(request.Id);

        if (categoriesWithAncestors.Count == 0)
            return Result.Fail(new CategoryNotFoundError());

        var targetCategory = categoriesWithAncestors.First();
        
        var ancestors = categoriesWithAncestors.Skip(1).Select(c => new CategoryResponse
        {
            id = c.Id,
            name = c.Name,
        }).ToList();

        var response = new CategoryTreeResponse
        {
            id = targetCategory.Id,
            name = targetCategory.Name,
            ancestors = ancestors
        };

        return Result.Ok(response);
    }
}
