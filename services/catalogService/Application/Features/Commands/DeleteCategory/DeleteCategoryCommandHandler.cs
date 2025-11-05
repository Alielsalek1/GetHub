using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(ICategoryCommandRepository categoryCommandRepository, ICategoryQueryRepository categoryQueryRepository)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryQueryRepository.GetByIdAsync(request.Id);
        if (category is null)
            return Result.Fail(new CategoryNotFoundError());

        // check if category has child categories
        if (await categoryQueryRepository.HasChildCategoriesAsync(request.Id))
            return Result.Fail(new InvalidCategoryDeleteError());

        await categoryCommandRepository.DeleteAsync(category);
        return Result.Ok();
    }
}