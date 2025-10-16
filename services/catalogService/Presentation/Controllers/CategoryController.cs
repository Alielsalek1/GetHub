using System.Runtime.ExceptionServices;
using CatalogService.Application.DTOs;
using CatalogService.Application.Features.Commands.CreateCategory;
using CatalogService.Application.Features.Commands.UpdateCategory;
using CatalogService.Application.Features.Queries.GetBaseCategories;
using CatalogService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Extensions;

namespace CatalogService.Presentation.controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IResult> GetCategoryById(int id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Category retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpGet("bases")]
    public async Task<IResult> GetBaseCategories()
    {
        var query = new GetBaseCategoriesQuery();
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Base categories retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpGet("firstLevel/{id}")]
    public async Task<IResult> GetFirstLevelCategories(int id)
    {
        var query = new GetFirstLevelCategoriesQuery(id);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "First level categories retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpGet("tree/{id}")]
    public async Task<IResult> GetCategoryTree(int id)
    {
        var query = new GetCategoryTreeByIdQuery(id);
        var result = await mediator.Send(query);
        return result.ToApiResult(
            successMessage: "Category tree with ancestors retrieved successfully",
            successStatusCode: 200
        );
    }

    [HttpPost]
    public async Task<IResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.Name,
            ParentId = request.ParentId
        };

        var result = await mediator.Send(command);
        return result.ToApiResult(
            successMessage: "Category created successfully",
            successStatusCode: 201
        );
    }

    [HttpPut("{id}")]
    public async Task<IResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,   
            Name = request.Name, 
            ParentId = request.ParentId
        };
        
        var result = await mediator.Send(command);
        return result.ToApiResult(
            successMessage: "Category updated successfully",
            successStatusCode: 200
        );
    }
}