using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Result>
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
}
