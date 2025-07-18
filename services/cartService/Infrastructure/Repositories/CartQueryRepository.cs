using Microsoft.EntityFrameworkCore;
using cartService.Application.Queries.Interfaces;
using cartService.Application.Queries.Models;
using cartService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace cartService.Infrastructure.Repositories;

public class CartQueryRepository : ICartQueryRepository
{
    private readonly CartReadDbContext _context;
    private readonly CartDbContext _writeContext; // For direct queries when read model isn't available

    public CartQueryRepository(CartReadDbContext context, CartDbContext writeContext)
    {
        _context = context;
        _writeContext = writeContext;
    }

    public async Task<Result<CartDetailsDto>> GetCartDetailsByCustomerAsync(Guid customerId)
    {
        try
        {
            // First try to get from read model
            var cartDetails = await _context.CartDetails
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (cartDetails != null)
                return Result<CartDetailsDto>.Success(cartDetails);

            // Fallback to write model if read model not available
            var cart = await _writeContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (cart == null)
                return Result<CartDetailsDto>.Failure("Cart not found for customer");

            var cartDetailsDto = new CartDetailsDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                IsActive = cart.IsActive,
                TotalAmount = cart.TotalAmount,
                TotalItems = cart.TotalItems,
                Items = cart.Items.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    AddedAt = item.AddedAt,
                    UpdatedAt = item.UpdatedAt
                }).ToList()
            };

            return Result<CartDetailsDto>.Success(cartDetailsDto);
        }
        catch (Exception ex)
        {
            return Result<CartDetailsDto>.Failure($"Error retrieving cart details: {ex.Message}");
        }
    }

    public async Task<Result<CartSummaryDto>> GetCartSummaryByCustomerAsync(Guid customerId)
    {
        try
        {
            // First try to get from read model
            var cartSummary = await _context.CartSummaries
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (cartSummary != null)
                return Result<CartSummaryDto>.Success(cartSummary);

            // Fallback to write model if read model not available
            var cart = await _writeContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (cart == null)
                return Result<CartSummaryDto>.Failure("Cart not found for customer");

            var cartSummaryDto = new CartSummaryDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                UpdatedAt = cart.UpdatedAt,
                TotalAmount = cart.TotalAmount,
                TotalItems = cart.TotalItems,
                IsActive = cart.IsActive
            };

            return Result<CartSummaryDto>.Success(cartSummaryDto);
        }
        catch (Exception ex)
        {
            return Result<CartSummaryDto>.Failure($"Error retrieving cart summary: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<CartSummaryDto>>> GetActiveCartsAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var summaries = await _context.CartSummaries
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<IEnumerable<CartSummaryDto>>.Success(summaries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CartSummaryDto>>.Failure($"Error retrieving active carts: {ex.Message}");
        }
    }
}
