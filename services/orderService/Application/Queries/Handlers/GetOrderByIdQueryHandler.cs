using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using orderService.Application.Queries;
using orderService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace orderService.Application.Queries.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderReadModel>>
{
    private readonly OrderReadDbContext _context;

    public GetOrderByIdQueryHandler(OrderReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderReadModel>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
            return Result.Fail(new NotFoundError($"Order {request.OrderId} not found"));

        var readModel = new OrderReadModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            BillingAddress = order.BillingAddress,
            Items = order.Items.Select(i => new OrderItemReadModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        return Result.Ok(readModel);
    }
}
