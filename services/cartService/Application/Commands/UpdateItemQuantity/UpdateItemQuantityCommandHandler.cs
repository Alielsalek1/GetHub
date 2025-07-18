using cartService.Application.Commands.UpdateItemQuantity;
using cartService.Domain.Events;
using cartService.Domain.Interfaces;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.UpdateItemQuantity;

public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, Result<bool>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateItemQuantityCommandHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
    {
        _cartRepository = cartRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.NewQuantity <= 0)
                return Result<bool>.Failure("Quantity must be greater than zero");

            // Get cart for customer
            var cartResult = await _cartRepository.GetByCustomerIdAsync(request.CustomerId);
            if (!cartResult.IsSuccess)
                return Result<bool>.Failure("Cart not found for customer");

            var cart = cartResult.Value!;
            var item = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);
            
            if (item == null)
                return Result<bool>.Failure("Item not found in cart");

            var oldQuantity = item.Quantity;

            // Update item quantity
            cart.UpdateItemQuantity(request.ProductId, request.NewQuantity);

            // Update cart
            var updateResult = await _cartRepository.UpdateAsync(cart);
            if (!updateResult.IsSuccess)
                return Result<bool>.Failure($"Failed to update cart: {updateResult.Error}");

            // Publish quantity updated event
            await _publishEndpoint.Publish(new ItemQuantityUpdated(
                cart.Id,
                cart.CustomerId,
                request.ProductId,
                oldQuantity,
                request.NewQuantity,
                DateTime.UtcNow
            ), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error updating item quantity: {ex.Message}");
        }
    }
}
