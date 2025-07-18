using cartService.Application.Commands.AddItemToCart;
using cartService.Application.Commands.RemoveItemFromCart;
using cartService.Application.Commands.UpdateItemQuantity;
using cartService.Application.Commands.ClearCart;
using cartService.Application.Queries.GetCartByCustomer;
using cartService.Application.Queries.GetCartSummary;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SharedKernel.Extensions;

namespace cartService.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/cart")
            .WithTags("Cart")
            .RequireAuthorization();

        // Commands
        group.MapPost("/items", AddItemToCart)
            .WithName("AddItemToCart")
            .WithSummary("Add item to cart")
            .WithDescription("Adds an item to the customer's cart or updates quantity if item already exists");

        group.MapPut("/items/{productId}/quantity", UpdateItemQuantity)
            .WithName("UpdateItemQuantity")
            .WithSummary("Update item quantity")
            .WithDescription("Updates the quantity of a specific item in the cart");

        group.MapDelete("/items/{productId}", RemoveItemFromCart)
            .WithName("RemoveItemFromCart")
            .WithSummary("Remove item from cart")
            .WithDescription("Removes a specific item from the cart");

        group.MapDelete("/clear", ClearCart)
            .WithName("ClearCart")
            .WithSummary("Clear cart")
            .WithDescription("Removes all items from the customer's cart");

        // Queries
        group.MapGet("/customer/{customerId}", GetCartByCustomer)
            .WithName("GetCartByCustomer")
            .WithSummary("Get cart by customer")
            .WithDescription("Retrieves the full cart details for a specific customer");

        group.MapGet("/customer/{customerId}/summary", GetCartSummary)
            .WithName("GetCartSummary")
            .WithSummary("Get cart summary")
            .WithDescription("Retrieves a summary of the cart for a specific customer");
    }

    private static async Task<IResult> AddItemToCart(
        AddItemToCartCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateItemQuantity(
        Guid productId,
        UpdateItemQuantityCommand command,
        IMediator mediator)
    {
        // Ensure the product ID in the route matches the command
        if (productId != command.ProductId)
        {
            return Results.BadRequest("Product ID mismatch");
        }

        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> RemoveItemFromCart(
        Guid productId,
        Guid customerId,
        IMediator mediator)
    {
        var command = new RemoveItemFromCartCommand 
        { 
            CustomerId = customerId,
            ProductId = productId 
        };
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> ClearCart(
        Guid customerId,
        IMediator mediator)
    {
        var command = new ClearCartCommand { CustomerId = customerId };
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetCartByCustomer(
        Guid customerId,
        IMediator mediator)
    {
        var query = new GetCartByCustomerQuery { CustomerId = customerId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetCartSummary(
        Guid customerId,
        IMediator mediator)
    {
        var query = new GetCartSummaryQuery { CustomerId = customerId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }
}
