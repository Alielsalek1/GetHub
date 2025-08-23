using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Queries;

public record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryResponse>>
{}