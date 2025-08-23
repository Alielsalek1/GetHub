using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Queries.GetCategoryById;

public class GetCategoryByIdHandler(ICategoryRepository categoryRepository) 
: IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        
        if (category == null)
            return Result.Fail(new NotFoundError($"Category with ID {request.Id} not found"));

        return Result.Ok(new CategoryResponse
        {
            id = category.Id,
            name = category.Name
        });
    }
}
