namespace orderService.Domain.Events;

public record OrderCreated(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    List<OrderItemDto> Items,
    DateTime CreatedAt
);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);
