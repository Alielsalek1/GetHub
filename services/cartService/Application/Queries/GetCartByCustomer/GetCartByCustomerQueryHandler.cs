using cartService.Application.Queries.GetCartByCustomer;
using cartService.Application.Queries.Interfaces;
using cartService.Application.Queries.Models;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Queries.GetCartByCustomer;

public class GetCartByCustomerQueryHandler : IRequestHandler<GetCartByCustomerQuery, Result<CartDetailsDto>>
{
    private readonly ICartQueryRepository _cartQueryRepository;

    public GetCartByCustomerQueryHandler(ICartQueryRepository cartQueryRepository)
    {
        _cartQueryRepository = cartQueryRepository;
    }

    public async Task<Result<CartDetailsDto>> Handle(GetCartByCustomerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _cartQueryRepository.GetCartDetailsByCustomerAsync(request.CustomerId);
        }
        catch (Exception ex)
        {
            return Result<CartDetailsDto>.Failure($"Error retrieving cart: {ex.Message}");
        }
    }
}
