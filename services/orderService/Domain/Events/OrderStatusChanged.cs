namespace orderService.Domain.Events;

public record OrderStatusChanged(
    Guid OrderId,
    string OldStatus,
    string NewStatus,
    DateTime ChangedAt
);
