using FluentResults;
using MediatR;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Features.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id);
        
        if (category == null)
            return Result.Fail($"Category with ID {request.Id} not found");

        if (!string.IsNullOrWhiteSpace(request.Name))
            category.Name = request.Name;
        if (request.ParentId.HasValue)
            category.ParentId = request.ParentId.Value;

        await categoryRepository.UpdateAsync(category);

        return Result.Ok();
    }
}