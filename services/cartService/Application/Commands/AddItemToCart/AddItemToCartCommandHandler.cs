using cartService.Application.Commands.AddItemToCart;
using cartService.Domain.Entities;
using cartService.Domain.Events;
using cartService.Domain.Interfaces;
using MassTransit;
using MediatR;
using SharedKernel.ResultPattern;

namespace cartService.Application.Commands.AddItemToCart;

public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Result<bool>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddItemToCartCommandHandler(ICartRepository cartRepository, IPublishEndpoint publishEndpoint)
    {
        _cartRepository = cartRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<bool>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            if (request.Quantity <= 0)
                return Result<bool>.Failure("Quantity must be greater than zero");

            if (request.UnitPrice <= 0)
                return Result<bool>.Failure("Unit price must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.ProductName))
                return Result<bool>.Failure("Product name is required");

            // Get or create cart for customer
            var cartResult = await _cartRepository.GetByCustomerIdAsync(request.CustomerId);
            Cart cart;

            if (cartResult.IsSuccess)
            {
                cart = cartResult.Value!;
            }
            else
            {
                // Create new cart for customer
                cart = new Cart(request.CustomerId);
                var createResult = await _cartRepository.CreateAsync(cart);
                if (!createResult.IsSuccess)
                    return Result<bool>.Failure($"Failed to create cart: {createResult.Error}");

                // Publish cart created event
                await _publishEndpoint.Publish(new CartCreated(
                    cart.Id,
                    cart.CustomerId,
                    cart.CreatedAt
                ), cancellationToken);
            }

            // Add item to cart
            cart.AddItem(request.ProductId, request.ProductName, request.UnitPrice, request.Quantity);

            // Update cart
            var updateResult = await _cartRepository.UpdateAsync(cart);
            if (!updateResult.IsSuccess)
                return Result<bool>.Failure($"Failed to update cart: {updateResult.Error}");

            // Publish item added event
            await _publishEndpoint.Publish(new ItemAddedToCart(
                cart.Id,
                cart.CustomerId,
                request.ProductId,
                request.ProductName,
                request.Quantity,
                request.UnitPrice,
                DateTime.UtcNow
            ), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error adding item to cart: {ex.Message}");
        }
    }
}
