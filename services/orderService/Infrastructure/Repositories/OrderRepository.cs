using Microsoft.EntityFrameworkCore;
using orderService.Domain.Entities;
using orderService.Domain.Interfaces;
using orderService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace orderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Order>> GetByIdAsync(Guid orderId)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return Result<Order>.Failure("Order not found");

            return Result<Order>.Success(order);
        }
        catch (Exception ex)
        {
            return Result<Order>.Failure($"Error retrieving order: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Order>>> GetByCustomerIdAsync(Guid customerId)
    {
        try
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Result<IEnumerable<Order>>.Success(orders);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Order>>.Failure($"Error retrieving customer orders: {ex.Message}");
        }
    }

    public async Task<Result<Order>> AddAsync(Order order)
    {
        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Result<Order>.Success(order);
        }
        catch (Exception ex)
        {
            return Result<Order>.Failure($"Error adding order: {ex.Message}");
        }
    }

    public async Task<Result<Order>> UpdateAsync(Order order)
    {
        try
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Result<Order>.Success(order);
        }
        catch (Exception ex)
        {
            return Result<Order>.Failure($"Error updating order: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid orderId)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return Result.Failure("Order not found");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting order: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid orderId)
    {
        try
        {
            var exists = await _context.Orders.AnyAsync(o => o.Id == orderId);
            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking order existence: {ex.Message}");
        }
    }
}
