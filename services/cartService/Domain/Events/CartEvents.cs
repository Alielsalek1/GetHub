namespace cartService.Domain.Events;

public record CartCreated(Guid CartId, Guid CustomerId, DateTime CreatedAt);

public record ItemAddedToCart(Guid CartId, Guid CustomerId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice, DateTime AddedAt);

public record ItemRemovedFromCart(Guid CartId, Guid CustomerId, Guid ProductId, string ProductName, DateTime RemovedAt);

public record ItemQuantityUpdated(Guid CartId, Guid CustomerId, Guid ProductId, int OldQuantity, int NewQuantity, DateTime UpdatedAt);

public record CartCleared(Guid CartId, Guid CustomerId, DateTime ClearedAt);

public record CartAbandoned(Guid CartId, Guid CustomerId, DateTime AbandonedAt, decimal TotalAmount, int TotalItems);
