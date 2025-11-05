using FluentResults;
using MediatR;

namespace CatalogService.Application.Features.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<Result>;