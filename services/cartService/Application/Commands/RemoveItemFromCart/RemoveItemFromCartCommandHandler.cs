using cartService.Application.Commands.RemoveItemFromCart;
using cartService.Domain.Events;
using cartService.Domain.Interfaces;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.RemoveItemFromCart;

public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, Result<bool>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RemoveItemFromCartCommandHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
    {
        _cartRepository = cartRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get cart for customer
            var cartResult = await _cartRepository.GetByCustomerIdAsync(request.CustomerId);
            if (!cartResult.IsSuccess)
                return Result<bool>.Failure("Cart not found for customer");

            var cart = cartResult.Value!;
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
            
            if (item == null)
                return Result<bool>.Failure("Item not found in cart");

            // Store item info for event
            var productName = item.ProductName;

            // Remove item from cart
            cart.RemoveItem(request.ProductId);

            // Update cart
            var updateResult = await _cartRepository.UpdateAsync(cart);
            if (!updateResult.IsSuccess)
                return Result<bool>.Failure($"Failed to update cart: {updateResult.Error}");

            // Publish item removed event
            await _publishEndpoint.Publish(new ItemRemovedFromCart(
                cart.Id,
                cart.CustomerId,
                request.ProductId,
                productName,
                DateTime.UtcNow
            ), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error removing item from cart: {ex.Message}");
        }
    }
}
