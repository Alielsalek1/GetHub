using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Commands;

public record CreateCategoryCommand : IRequest<Result>
{
    public string name { get; set; } = null!;
    public int parentId { get; set; }
}
