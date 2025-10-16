using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Queries.GetProductsByBrand;

public record GetProductsByBrandQuery(string Brand) : IRequest<Result<List<ProductResponse>>>;
