using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<int>>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public int CategoryId { get; set; }
}
