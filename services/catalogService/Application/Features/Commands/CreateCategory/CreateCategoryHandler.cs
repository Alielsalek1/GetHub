using CatalogService.Application.Features.Commands.CreateCategory;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using SharedKernel;

namespace CatalogService.Application.Features.Commands.CreateCategory;

public class CreateCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, Result>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Check if category with same name already exists
        var existingCategory = await categoryRepository.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            return Result.Fail(new AlreadyExistsError($"Category with name '{request.Name}' already exists"));
        }

        // Validate parent category exists if ParentId is provided
        if (request.ParentId.HasValue)
        {
            var parentCategory = await categoryRepository.GetByIdAsync(request.ParentId.Value);
            if (parentCategory == null)
            {
                return Result.Fail(new NotFoundError($"Parent category with ID {request.ParentId} not found"));
            }
        }

        var category = new Category
        {
            Name = request.Name,
            ParentId = request.ParentId
        };

        await categoryRepository.AddAsync(category);

        return Result.Ok();
    }
}