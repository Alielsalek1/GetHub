using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Queries.GetCategoryTreeById;

public class GetCategoryTreeByIdHandler(ICategoryRepository categoryRepository) 
    : IRequestHandler<GetCategoryTreeByIdQuery, Result<CategoryTreeResponse>>
{
    public async Task<Result<CategoryTreeResponse>> Handle(GetCategoryTreeByIdQuery request, CancellationToken cancellationToken)
    {
        var categoriesWithAncestors = await categoryRepository.GetCategoryWithAncestorsAsync(request.Id);

        if (categoriesWithAncestors.Count == 0)
            return Result.Fail(new NotFoundError($"Category with ID {request.Id} not found"));

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
