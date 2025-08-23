using CatalogService.Application.DTOs;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Queries;

public record GetCategoryTreeByIdQuery(int Id) : IRequest<Result<CategoryTreeResponse>>
{}