using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Queries.GetFirstLevelCategories;

public class GetFirstLevelCategoriesHandler(ICategoryRepository categoryRepository) :
    IRequestHandler<GetFirstLevelCategoriesQuery, Result<List<CategoryResponse>>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetFirstLevelCategoriesQuery request, CancellationToken cancellationToken)
    {
        var ParentCategory = await categoryRepository.GetByIdAsync(request.Id);

        if (ParentCategory is null)
            return Result.Fail(new NotFoundError($"Parent category with ID {request.Id} not found"));

        var FirstLevelCategories = await categoryRepository.GetFirstLevelCategoriesAsync(request.Id);

        return Result.Ok(FirstLevelCategories.Select(c => new CategoryResponse
        {
            id = c.Id,
            name = c.Name
        }).ToList());
    }
}