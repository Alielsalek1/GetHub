using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Queries;

public record GetFirstLevelCategoriesQuery(int Id) : IRequest<Result<List<CategoryResponse>>>
{}