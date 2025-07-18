using cartService.Domain.Entities;
using SharedKernel.ResultPattern;

namespace cartService.Domain.Interfaces;

public interface ICartRepository
{
    Task<Result<Cart>> GetByIdAsync(Guid cartId);
    Task<Result<Cart>> GetByCustomerIdAsync(Guid customerId);
    Task<Result<Cart>> CreateAsync(Cart cart);
    Task<Result<Cart>> UpdateAsync(Cart cart);
    Task<Result<bool>> DeleteAsync(Guid cartId);
    Task<Result<bool>> ExistsAsync(Guid cartId);
}
