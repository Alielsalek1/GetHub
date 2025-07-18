using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using orderService.Application.Queries;
using orderService.Infrastructure.Persistence;

namespace orderService.Application.Queries.Handlers;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, Result<List<OrderSummaryReadModel>>>
{
    private readonly OrderReadDbContext _context;

    public GetOrdersByCustomerQueryHandler(OrderReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<OrderSummaryReadModel>>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == request.CustomerId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderSummaryReadModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Count
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(orders);
    }
}
