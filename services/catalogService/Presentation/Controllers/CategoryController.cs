using System.Runtime.ExceptionServices;
using CatalogService.Application.DTOs;
using CatalogService.Application.Features.Commands.CreateCategory;
using CatalogService.Application.Features.Commands.UpdateCategory;
using CatalogService.Application.Features.Queries.GetBaseCategories;
using CatalogService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Annotations;
using Shared.Enums;
using CatalogService.Application.Features.Commands.DeleteCategory;

namespace CatalogService.Presentation.controllers;

[ApiController]
[Route("api/category")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Category retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpGet("bases")]
    public async Task<IActionResult> GetBaseCategories()
    {
        var query = new GetBaseCategoriesQuery();
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Base categories retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpGet("firstLevelChildren/{id}")]
    public async Task<IActionResult> GetFirstLevelCategories(int id)
    {
        var query = new GetFirstLevelCategoriesQuery(id);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "First level categories retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpGet("ancestorsTree/{id}")]
    public async Task<IActionResult> GetCategoryTree(int id)
    {
        var query = new GetCategoryTreeByIdQuery(id);
        var result = await mediator.Send(query);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 200,
                successMessage: "Category tree with ancestors retrieved successfully");
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpPost]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.name,
            ParentId = request.parentId
        };

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 201);
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpPut("{id}")]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Name = request.name,
            ParentId = request.parentId
        };

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 204);
        return ResultExtensions.ToErrorApiResult(result);
    }

    [HttpDelete("{id}")]
    [AuthorizeAuthType(AuthType.Manager)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var command = new DeleteCategoryCommand(id);

        var result = await mediator.Send(command);
        if (result.IsSuccess)
            return ResultExtensions.ToSuccessApiResult(result,
                successStatusCode: 204);
        return ResultExtensions.ToErrorApiResult(result);
    }
}