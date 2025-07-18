using orderService.Application.Commands.CreateOrder;
using orderService.Application.Commands.UpdateOrderStatus;
using orderService.Application.Commands.CancelOrder;
using orderService.Application.Queries.GetOrderById;
using orderService.Application.Queries.GetOrdersByCustomer;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SharedKernel.Extensions;

namespace orderService.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders")
            .RequireAuthorization();

        // Commands
        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithSummary("Create a new order")
            .WithDescription("Creates a new order with the provided details");

        group.MapPut("/{orderId}/status", UpdateOrderStatus)
            .WithName("UpdateOrderStatus")
            .WithSummary("Update order status")
            .WithDescription("Updates the status of an existing order");

        group.MapDelete("/{orderId}", CancelOrder)
            .WithName("CancelOrder")
            .WithSummary("Cancel an order")
            .WithDescription("Cancels an existing order");

        // Queries
        group.MapGet("/{orderId}", GetOrderById)
            .WithName("GetOrderById")
            .WithSummary("Get order by ID")
            .WithDescription("Retrieves order details by order ID");

        group.MapGet("/customer/{customerId}", GetOrdersByCustomer)
            .WithName("GetOrdersByCustomer")
            .WithSummary("Get orders by customer")
            .WithDescription("Retrieves all orders for a specific customer");
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateOrderStatus(
        Guid orderId,
        UpdateOrderStatusCommand command,
        IMediator mediator)
    {
        // Ensure the order ID in the route matches the command
        if (orderId != command.OrderId)
        {
            return Results.BadRequest("Order ID mismatch");
        }

        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> CancelOrder(
        Guid orderId,
        IMediator mediator)
    {
        var command = new CancelOrderCommand { OrderId = orderId };
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetOrderById(
        Guid orderId,
        IMediator mediator)
    {
        var query = new GetOrderByIdQuery { OrderId = orderId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetOrdersByCustomer(
        Guid customerId,
        IMediator mediator)
    {
        var query = new GetOrdersByCustomerQuery { CustomerId = customerId };
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }
}
