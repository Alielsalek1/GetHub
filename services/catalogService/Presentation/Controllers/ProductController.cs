using CatalogService.Application.DTOs;
using CatalogService.Application.Features.Commands.CreateProduct;
using CatalogService.Application.Features.Commands.UpdateProduct;
using CatalogService.Application.Features.Queries.GetProductById;
using CatalogService.Application.Features.Queries.GetProductByName;
using CatalogService.Application.Features.Queries.GetProductsByBrand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Extensions;

namespace CatalogService.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IResult> GetProductById(int id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Product retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpGet("name/{name}")]
    public async Task<IResult> GetProductByName(string name)
    {
        var query = new GetProductByNameQuery(name);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Product retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpGet("brand/{brand}")]
    public async Task<IResult> GetProductsByBrand(string brand)
    {
        var query = new GetProductsByBrandQuery(brand);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Products retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpPost]
    public async Task<IResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            CategoryId = request.CategoryId
        };

        var result = await mediator.Send(command);
        return result.ToApiResult(
            successMessage: "Product created successfully",
            successStatusCode: 201
        );
    }

    [HttpPut("{id}")]
    public async Task<IResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
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
        return result.ToApiResult(
            successMessage: "Product updated successfully",
            successStatusCode: 200
        );
    }
}
