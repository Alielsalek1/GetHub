using MediatR;
using FluentResults;
using MassTransit;
using orderService.Application.Commands;
using orderService.Domain.Entities;
using orderService.Domain.Events;
using orderService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace orderService.Application.Commands.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public CreateOrderCommandHandler(OrderDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            return Result.Fail(new ValidationError("Order must contain at least one item"));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            ShippingAddress = request.ShippingAddress,
            BillingAddress = request.BillingAddress,
            Status = OrderStatus.Pending
        };

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
                return Result.Fail(new ValidationError($"Invalid quantity for product {item.ProductName}"));

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var orderCreatedEvent = new OrderCreated(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList(),
            order.CreatedAt
        );

        await _publisher.Publish(orderCreatedEvent, cancellationToken);

        return Result.Ok(order.Id);
    }
}
