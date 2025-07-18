using MediatR;
using FluentResults;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using orderService.Application.Commands;
using orderService.Domain.Entities;
using orderService.Domain.Events;
using orderService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace orderService.Application.Commands.Handlers;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly OrderDbContext _context;
    private readonly IPublishEndpoint _publisher;

    public UpdateOrderStatusCommandHandler(OrderDbContext context, IPublishEndpoint publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(request.OrderId);
        if (order == null)
            return Result.Fail(new NotFoundError($"Order {request.OrderId} not found"));

        if (order.Status == OrderStatus.Cancelled)
            return Result.Fail(new ValidationError("Cannot update status of cancelled order"));

        if (order.Status == OrderStatus.Delivered)
            return Result.Fail(new ValidationError("Cannot update status of delivered order"));

        var oldStatus = order.Status.ToString();
        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var statusChangedEvent = new OrderStatusChanged(
            order.Id,
            oldStatus,
            request.NewStatus.ToString(),
            order.UpdatedAt
        );

        await _publisher.Publish(statusChangedEvent, cancellationToken);

        return Result.Ok();
    }
}
