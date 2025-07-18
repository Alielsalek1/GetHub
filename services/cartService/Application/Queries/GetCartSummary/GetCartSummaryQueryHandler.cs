using cartService.Application.Queries.GetCartSummary;
using cartService.Application.Queries.Interfaces;
using cartService.Application.Queries.Models;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Queries.GetCartSummary;

public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, Result<CartSummaryDto>>
{
    private readonly ICartQueryRepository _cartQueryRepository;

    public GetCartSummaryQueryHandler(ICartQueryRepository cartQueryRepository)
    {
        _cartQueryRepository = cartQueryRepository;
    }

    public async Task<Result<CartSummaryDto>> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _cartQueryRepository.GetCartSummaryByCustomerAsync(request.CustomerId);
        }
        catch (Exception ex)
        {
            return Result<CartSummaryDto>.Failure($"Error retrieving cart summary: {ex.Message}");
        }
    }
}
