using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Queries.GetProductByName;

public record GetProductByNameQuery(string Name) : IRequest<Result<ProductResponse>>;
