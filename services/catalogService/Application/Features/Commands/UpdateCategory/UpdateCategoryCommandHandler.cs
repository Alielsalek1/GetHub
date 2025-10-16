using FluentResults;
using MediatR;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using SharedKernel;

namespace CatalogService.Application.Features.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        
        if (category == null)
        {
            return Result.Fail(new NotFoundError($"Category with ID {request.Id} not found"));
        }

        // Check for name conflicts if changing name
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != category.Name)
        {
            var existingCategory = await categoryRepository.GetByNameAsync(request.Name);
            if (existingCategory != null && existingCategory.Id != request.Id)
            {
                return Result.Fail(new AlreadyExistsError($"Category with name '{request.Name}' already exists"));
            }
        }

        // Validate parent category exists if changing parent
        if (request.ParentId.HasValue && request.ParentId != category.ParentId)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(request.ParentId.Value);
            if (parentCategory == null)
                return Result.Fail(new NotFoundError($"Parent category with ID {request.ParentId} not found"));

            // Prevent circular reference (category cannot be its own parent)
            if (request.ParentId == request.Id)
                return Result.Fail(new ValidationError("Category cannot be its own parent"));

            // Check for circular reference in the hierarchy
            var ancestors = await categoryRepository.GetCategoryWithAncestorsAsync(request.ParentId.Value);
            if (ancestors.Any(a => a.Id == request.Id))
                return Result.Fail(new ValidationError("Cannot create circular reference in category hierarchy"));
        }

        // Create updated category entity
        var updatedCategory = new Category
        {
            Id = request.Id,
            Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : category.Name,
            ParentId = request.ParentId ?? category.ParentId
        };

        await categoryRepository.UpdateAsync(updatedCategory);

        return Result.Ok();
    }
}