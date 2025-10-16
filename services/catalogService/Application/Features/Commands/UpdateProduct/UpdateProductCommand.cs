using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Result>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public int? CategoryId { get; set; }
}
