using cartService.Application.Commands.ClearCart;
using cartService.Domain.Events;
using cartService.Domain.Interfaces;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ClearCartCommandHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
    {
        _cartRepository = cartRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get cart for customer
            var cartResult = await _cartRepository.GetByCustomerIdAsync(request.CustomerId);
            if (!cartResult.IsSuccess)
                return Result<bool>.Failure("Cart not found for customer");

            var cart = cartResult.Value!;

            // Clear cart
            cart.ClearCart();

            // Update cart
            var updateResult = await _cartRepository.UpdateAsync(cart);
            if (!updateResult.IsSuccess)
                return Result<bool>.Failure($"Failed to clear cart: {updateResult.Error}");

            // Publish cart cleared event
            await _publishEndpoint.Publish(new CartCleared(
                cart.Id,
                cart.CustomerId,
                DateTime.UtcNow
            ), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error clearing cart: {ex.Message}");
        }
    }
}
