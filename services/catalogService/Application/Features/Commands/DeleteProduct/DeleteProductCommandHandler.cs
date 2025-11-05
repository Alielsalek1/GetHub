using CatalogService.Application.Interfaces;
using CatalogService.Domain.models;
using FluentResults;
using MediatR;
using Shared;

namespace CatalogService.Application.Features.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductCommandRepository productCommandRepository, IProductQueryRepository productQueryRepository)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productQueryRepository.GetByIdAsync(request.Id);
        if (product is null)
            return Result.Fail(new ProductNotFoundError());

        await productCommandRepository.DeleteAsync(product);
        return Result.Ok();
    }
}