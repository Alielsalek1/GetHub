using CatalogService.Application.DTOs;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.DeleteProduct;
using CatalogService.Application.Features.Commands.UpdateProduct;
using CatalogService.Application.Features.Queries.GetProductById;
using CatalogService.Application.Features.Queries.GetProductByName;
using CatalogService.Application.Features.Queries.GetProductsByBrand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Annotations;
using Shared.Enums;
using Shared.Extensions;

namespace CatalogService.Presentation.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Product retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetProductByName(string name)
    {
        var query = new GetProductByNameQuery(name);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Product retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpGet("brand/{brand}")]
    public async Task<IActionResult> GetProductsByBrand(string brand)
    {
        var query = new GetProductsByBrandQuery(brand);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Products retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpPost]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            CategoryId = request.CategoryId
        };

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 201,
                successMessage: "Product created successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpPut("{id}")]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            CategoryId = request.CategoryId
        };

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Product updated successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpDelete("{id}")]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 204);
        return ResultExtensions.ToErrorApiResult(result);
    }
}