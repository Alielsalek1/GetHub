using Microsoft.EntityFrameworkCore;
using cartService.Domain.Entities;
using cartService.Domain.Interfaces;
using cartService.Infrastructure.Persistence;
using SharedKernel.ResultPattern;

namespace cartService.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly CartDbContext _context;

    public CartRepository(CartDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Cart>> GetByIdAsync(Guid cartId)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return Result<Cart>.Failure("Cart not found");

            return Result<Cart>.Success(cart);
        }
        catch (Exception ex)
        {
            return Result<Cart>.Failure($"Error retrieving cart: {ex.Message}");
        }
    }

    public async Task<Result<Cart>> GetByCustomerIdAsync(Guid customerId)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

            if (cart == null)
                return Result<Cart>.Failure("Active cart not found for customer");

            return Result<Cart>.Success(cart);
        }
        catch (Exception ex)
        {
            return Result<Cart>.Failure($"Error retrieving cart by customer: {ex.Message}");
        }
    }

    public async Task<Result<Cart>> CreateAsync(Cart cart)
    {
        try
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return Result<Cart>.Success(cart);
        }
        catch (Exception ex)
        {
            return Result<Cart>.Failure($"Error creating cart: {ex.Message}");
        }
    }

    public async Task<Result<Cart>> UpdateAsync(Cart cart)
    {
        try
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return Result<Cart>.Success(cart);
        }
        catch (Exception ex)
        {
            return Result<Cart>.Failure($"Error updating cart: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid cartId)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
                return Result<bool>.Failure("Cart not found");

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error deleting cart: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid cartId)
    {
        try
        {
            var exists = await _context.Carts.AnyAsync(c => c.Id == cartId);
            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking cart existence: {ex.Message}");
        }
    }
}
