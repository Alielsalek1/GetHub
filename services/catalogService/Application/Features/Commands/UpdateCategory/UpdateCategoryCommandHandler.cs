using FluentResults;
using MediatR;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using Shared;

namespace CatalogService.Application.Features.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryCommandRepository categoryCommandRepository, ICategoryQueryRepository categoryQueryRepository) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryQueryRepository.GetByIdAsync(request.Id);
        
        if (category == null)
        {
            return Result.Fail(new CategoryNotFoundError());
        }

        // Check for name conflicts if changing name
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != category.Name)
        {
            var existingCategory = await categoryQueryRepository.GetByNameAsync(request.Name);
            if (existingCategory != null && existingCategory.Id != request.Id)
            {
                return Result.Fail(new CategoryWithSameNameAlreadyExistsError());
            }
        }

        // Validate parent category exists if changing parent
        if (request.ParentId.HasValue && request.ParentId != category.ParentId)
        {
            var parentCategory = await categoryQueryRepository.GetByIdAsync(request.ParentId.Value);
            if (parentCategory == null)
                return Result.Fail(new ParentCategoryNotFoundError());

            // Prevent circular reference (category cannot be its own parent)
            if (request.ParentId == request.Id)
                return Result.Fail(new CategoryCircularDependencyUpdateError());

            // Check for circular reference in the hierarchy
            var ancestors = await categoryQueryRepository.GetCategoryWithAncestorsAsync(request.ParentId.Value);
            if (ancestors.Any(a => a.Id == request.Id))
                return Result.Fail(new CategoryCircularDependencyUpdateError());
        }

        // Create updated category entity
        var updatedCategory = new Category
        {
            Id = request.Id,
            Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : category.Name,
            ParentId = request.ParentId ?? category.ParentId
        };

        await categoryCommandRepository.UpdateAsync(updatedCategory);

        return Result.Ok();
    }
}