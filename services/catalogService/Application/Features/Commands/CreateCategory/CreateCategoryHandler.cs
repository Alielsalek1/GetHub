using CatalogService.Application.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Handlers;

public class CreateCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, Result>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.name,
            ParentId = request.parentId
        };

        await categoryRepository.AddAsync(category);

        return Result.Ok();
    }
}