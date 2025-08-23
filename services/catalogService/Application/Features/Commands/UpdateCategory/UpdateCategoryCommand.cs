using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentId { get; set; }
}