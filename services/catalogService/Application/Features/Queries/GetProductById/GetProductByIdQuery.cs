using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponse>>;
