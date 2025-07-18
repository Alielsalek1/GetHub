using MediatR;
using FluentResults;

namespace orderService.Application.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemRequest> Items,
    string? ShippingAddress,
    string? BillingAddress
) : IRequest<Result<Guid>>;

public record CreateOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);
