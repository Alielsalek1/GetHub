using CatalogService.Application.Features.Commands.CreateCategory;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Commands.CreateCategory;

public class CreateCategoryHandler(ICategoryCommandRepository categoryCommandRepository, ICategoryQueryRepository categoryQueryRepository) : IRequestHandler<CreateCategoryCommand, Result>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category with same name already exists
        var existingCategory = await categoryQueryRepository.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            return Result.Fail(new CategoryAlreadyExistsError());
        }

        // Validate parent category exists if ParentId is provided
        if (request.ParentId.HasValue)
        {
            var parentCategory = await categoryQueryRepository.GetByIdAsync(request.ParentId.Value);
            if (parentCategory == null)
            {
                return Result.Fail(new ParentCategoryNotFoundError());
            }
        }

        var category = new Category
        {
            Name = request.Name,
            ParentId = request.ParentId
        };

    await categoryCommandRepository.AddAsync(category);

        return Result.Ok();
    }
}