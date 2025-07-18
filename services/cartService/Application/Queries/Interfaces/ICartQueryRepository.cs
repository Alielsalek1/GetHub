using cartService.Application.Queries.Models;
using SharedKernel.ResultPattern;

namespace cartService.Application.Queries.Interfaces;

public interface ICartQueryRepository
{
    Task<Result<CartDetailsDto>> GetCartDetailsByCustomerAsync(Guid customerId);
    Task<Result<CartSummaryDto>> GetCartSummaryByCustomerAsync(Guid customerId);
    Task<Result<IEnumerable<CartSummaryDto>>> GetActiveCartsAsync(int pageNumber = 1, int pageSize = 10);
}
