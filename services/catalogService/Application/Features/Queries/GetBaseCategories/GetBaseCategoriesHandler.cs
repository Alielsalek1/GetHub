using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Queries.GetBaseCategories;

public class GetBaseCategoriesHandler(ICategoryQueryRepository categoryRepository) 
    : IRequestHandler<GetBaseCategoriesQuery, Result<List<CategoryResponse>>>
{
    public async Task<Result<List<CategoryResponse>>> Handle(GetBaseCategoriesQuery request, CancellationToken cancellationToken)
    {
        var baseCategories = await categoryRepository.GetBaseCategoriesAsync();

        if (baseCategories == null || baseCategories.Count == 0)
            return Result.Ok(new List<CategoryResponse>());

        var response = baseCategories.Select(c => new CategoryResponse
        {
            id = c.Id,
            name = c.Name
        }).ToList();

        return Result.Ok(response);
    }
}
