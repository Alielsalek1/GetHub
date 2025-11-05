using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<Result>;