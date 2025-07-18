using Microsoft.EntityFrameworkCore;
using orderService.Application.Queries.Interfaces;
using orderService.Application.Queries.Models;
using orderService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace orderService.Infrastructure.Repositories;

public class OrderQueryRepository : IOrderQueryRepository
{
    private readonly OrderReadDbContext _context;

    public OrderQueryRepository(OrderReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderDetails>> GetOrderDetailsAsync(Guid orderId)
    {
        try
        {
            var orderDetails = await _context.OrderDetails
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (orderDetails == null)
                return Result<OrderDetails>.Failure("Order not found");

            return Result<OrderDetails>.Success(orderDetails);
        }
        catch (Exception ex)
        {
            return Result<OrderDetails>.Failure($"Error retrieving order details: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<OrderSummary>>> GetOrderSummariesByCustomerAsync(Guid customerId)
    {
        try
        {
            var summaries = await _context.OrderSummaries
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Result<IEnumerable<OrderSummary>>.Success(summaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderSummary>>.Failure($"Error retrieving order summaries: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<OrderSummary>>> GetOrderSummariesAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var summaries = await _context.OrderSummaries
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<OrderSummary>>.Success(summaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<OrderSummary>>.Failure($"Error retrieving order summaries: {ex.Message}");
        }
    }
}
