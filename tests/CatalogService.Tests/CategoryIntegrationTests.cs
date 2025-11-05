using System.Collections.Concurrent;
using CatalogService.Application.DTOs;
using CatalogService.Application.Features.Commands.CreateCategory;
using CatalogService.Application.Features.Commands.UpdateCategory;
using CatalogService.Application.Features.Commands.DeleteCategory;
using CatalogService.Application.Queries;
using CatalogService.Application.Features.Queries.GetBaseCategories;
using CatalogService.Application.Features.Queries.GetCategoryById;
using CatalogService.Application.Features.Queries.GetFirstLevelCategories;
using CatalogService.Application.Features.Queries.GetCategoryTreeById;
using CatalogService.Domain.models;
using CatalogService.Tests.Infrastructure;
using FluentAssertions;
using Shared.Enums;
using Xunit;

namespace CatalogService.Tests;

public class CategoryIntegrationTests
{
    private static (CreateCategoryHandler createHandler, 
                    UpdateCategoryCommandHandler updateHandler,
                    DeleteCategoryCommandHandler deleteHandler,
                    GetCategoryByIdHandler getByIdHandler,
                    GetBaseCategoriesHandler getBaseCategoriesHandler,
                    GetFirstLevelCategoriesHandler getFirstLevelHandler,
                    GetCategoryTreeByIdHandler getCategoryTreeHandler) BuildHandlers(
        ConcurrentDictionary<int, Category> store)
    {
        var commandRepo = new InMemoryCategoryCommandRepository(store);
        var queryRepo = new InMemoryCategoryQueryRepository(store);

        return (
            new CreateCategoryHandler(commandRepo, queryRepo),
            new UpdateCategoryCommandHandler(commandRepo, queryRepo),
            new DeleteCategoryCommandHandler(commandRepo, queryRepo),
            new GetCategoryByIdHandler(queryRepo),
            new GetBaseCategoriesHandler(queryRepo),
            new GetFirstLevelCategoriesHandler(queryRepo),
            new GetCategoryTreeByIdHandler(queryRepo)
        );
    }

    // Test: Happy path - category exists
    [Fact]
    public async Task GetCategoryById_ExistingCategory_ReturnsCategory()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var createResult = await handlers.createHandler.Handle(new CreateCategoryCommand 
        { 
            Name = "Electronics" 
        }, CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var result = await handlers.getByIdHandler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be("Electronics");
    }

    // Test: CategoryNotFoundError
    [Fact]
    public async Task GetCategoryById_NonExistingCategory_ReturnsCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.getByIdHandler.Handle(new GetCategoryByIdQuery(999), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_NOT_FOUND);
    }

    // Test: Happy path - returns base categories
    [Fact]
    public async Task GetBaseCategories_ExistingBaseCategories_ReturnsCategories()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Books" }, CancellationToken.None);

        var result = await handlers.getBaseCategoriesHandler.Handle(new GetBaseCategoriesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(c => c.name == "Electronics");
        result.Value.Should().Contain(c => c.name == "Books");
    }

    // Test: Happy path - category created successfully
    [Fact]
    public async Task CreateCategory_ValidData_CreatesCategory()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.createHandler.Handle(new CreateCategoryCommand 
        { 
            Name = "Electronics" 
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        store.Should().ContainSingle();
        store.Values.First().Name.Should().Be("Electronics");
    }

    // Test: CategoryAlreadyExistsError
    [Fact]
    public async Task CreateCategory_DuplicateName_ReturnsCategoryAlreadyExistsError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_ALREADY_EXISTS);
    }

    // Test: ParentCategoryNotFoundError
    [Fact]
    public async Task CreateCategory_InvalidParentId_ReturnsParentCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.createHandler.Handle(new CreateCategoryCommand 
        { 
            Name = "Laptops",
            ParentId = 999
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.PARENT_CATEGORY_NOT_FOUND);
    }

    // Test: Happy path - category deleted successfully
    [Fact]
    public async Task DeleteCategory_ExistingCategory_DeletesCategory()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var createResult = await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.deleteHandler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var getResult = await handlers.getByIdHandler.Handle(new GetCategoryByIdQuery(1), CancellationToken.None);
        getResult.IsFailed.Should().BeTrue();
    }

    // Test: InvalidCategoryDeleteError - has children
    [Fact]
    public async Task DeleteCategory_HasChildren_ReturnsInvalidCategoryDeleteError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var parent = await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Laptops", ParentId = 1 }, CancellationToken.None);

        var result = await handlers.deleteHandler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.INVALID_CATEGORY_DELETE);
    }

    // Test: CategoryNotFoundError for delete
    [Fact]
    public async Task DeleteCategory_NonExistingCategory_ReturnsCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.deleteHandler.Handle(new DeleteCategoryCommand(999), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_NOT_FOUND);
    }

    // Test: Happy path - update category name
    [Fact]
    public async Task UpdateCategory_ValidData_UpdatesCategory()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 1,
            Name = "Electronics & Gadgets" 
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var category = store.Values.First();
        category.Name.Should().Be("Electronics & Gadgets");
    }

    // Test: CategoryNotFoundError for update
    [Fact]
    public async Task UpdateCategory_NonExistingCategory_ReturnsCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 999,
            Name = "NonExistent" 
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_NOT_FOUND);
    }

    // Test: CategoryWithSameNameAlreadyExistsError
    [Fact]
    public async Task UpdateCategory_DuplicateName_ReturnsCategoryWithSameNameAlreadyExistsError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Books" }, CancellationToken.None);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 2,
            Name = "Electronics" 
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_WITH_SAME_NAME_EXISTS);
    }

    // Test: ParentCategoryNotFoundError for update
    [Fact]
    public async Task UpdateCategory_InvalidParentId_ReturnsParentCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 1,
            ParentId = 999 
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.PARENT_CATEGORY_NOT_FOUND);
    }

    // Test: CategoryCircularDependencyUpdateError - self parent
    [Fact]
    public async Task UpdateCategory_SelfParent_ReturnsCategoryCircularDependencyUpdateError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 1,
            ParentId = 1 
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_CIRCULAR_DEPENDENCY);
    }

    // Test: CategoryCircularDependencyUpdateError - circular hierarchy
    [Fact]
    public async Task UpdateCategory_CircularHierarchy_ReturnsCategoryCircularDependencyUpdateError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Computers", ParentId = 1 }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Laptops", ParentId = 2 }, CancellationToken.None);

        // Try to make Electronics a child of Laptops (which is its descendant)
        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 1,
            ParentId = 3 
        }, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_CIRCULAR_DEPENDENCY);
    }

    // Test: Happy path - get first level categories
    [Fact]
    public async Task GetFirstLevelCategories_ExistingParent_ReturnsChildren()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Laptops", ParentId = 1 }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Phones", ParentId = 1 }, CancellationToken.None);

        var result = await handlers.getFirstLevelHandler.Handle(new GetFirstLevelCategoriesQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(c => c.name == "Laptops");
        result.Value.Should().Contain(c => c.name == "Phones");
    }

    // Test: CategoryNotFoundError for first level categories
    [Fact]
    public async Task GetFirstLevelCategories_NonExistingParent_ReturnsCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.getFirstLevelHandler.Handle(new GetFirstLevelCategoriesQuery(999), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_NOT_FOUND);
    }

    // Test: Empty list when parent has no children
    [Fact]
    public async Task GetFirstLevelCategories_ParentWithNoChildren_ReturnsEmptyList()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.getFirstLevelHandler.Handle(new GetFirstLevelCategoriesQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // Test: Happy path - get category tree with ancestors
    [Fact]
    public async Task GetCategoryTree_ExistingCategory_ReturnsCategoryWithAncestors()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Computers", ParentId = 1 }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Laptops", ParentId = 2 }, CancellationToken.None);

        var result = await handlers.getCategoryTreeHandler.Handle(new GetCategoryTreeByIdQuery(3), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.name.Should().Be("Laptops");
    }

    // Test: CategoryNotFoundError for tree structure
    [Fact]
    public async Task GetCategoryTree_NonExistingRootCategory_ReturnsCategoryNotFoundError()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.getCategoryTreeHandler.Handle(new GetCategoryTreeByIdQuery(999), CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.CATEGORY_NOT_FOUND);
    }

    // Test: Base category (no parent) for tree
    [Fact]
    public async Task GetCategoryTree_BaseCategory_ReturnsCategoryWithoutParent()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);

        var result = await handlers.getCategoryTreeHandler.Handle(new GetCategoryTreeByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.name.Should().Be("Electronics");
    }

    // Test: Empty base categories list
    [Fact]
    public async Task GetBaseCategories_NoCategories_ReturnsEmptyList()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        var result = await handlers.getBaseCategoriesHandler.Handle(new GetBaseCategoriesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    // Test: Update only parent (not name)
    [Fact]
    public async Task UpdateCategory_UpdateParentOnly_UpdatesParent()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Laptops" }, CancellationToken.None);

        var result = await handlers.updateHandler.Handle(new UpdateCategoryCommand 
        { 
            Id = 2,
            ParentId = 1 
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var laptop = store.Values.First(c => c.Id == 2);
        laptop.ParentId.Should().Be(1);
    }

    // Test: Create category with valid parent
    [Fact]
    public async Task CreateCategory_WithValidParent_CreatesCategory()
    {
        var store = new ConcurrentDictionary<int, Category>();
        var handlers = BuildHandlers(store);

        await handlers.createHandler.Handle(new CreateCategoryCommand { Name = "Electronics" }, CancellationToken.None);
        
        var result = await handlers.createHandler.Handle(new CreateCategoryCommand 
        { 
            Name = "Laptops",
            ParentId = 1
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        store.Should().HaveCount(2);
        var laptop = store.Values.First(c => c.Name == "Laptops");
        laptop.ParentId.Should().Be(1);
    }
}
