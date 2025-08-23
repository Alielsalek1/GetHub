using CatalogService.Application.DTOs;
using CatalogService.Application.Interfaces;
using CatalogService.Application.Queries;
using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Queries.GetBaseCategories;

public class GetBaseCategoriesQuery : IRequest<Result<List<CategoryResponse>>>
{}
