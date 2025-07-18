using transactionService.Domain.Enums;

namespace transactionService.Domain.Events;

public record TransactionCreated(
    Guid TransactionId,
    Guid UserId,
    TransactionType Type,
    decimal Amount,
    string Currency,
    PaymentMethod PaymentMethod,
    string ReferenceNumber,
    DateTime CreatedAt);

public record TransactionCompleted(
    Guid TransactionId,
    Guid UserId,
    TransactionType Type,
    decimal Amount,
    string Currency,
    DateTime CompletedAt,
    string ReferenceNumber);

public record TransactionCancelled(
    Guid TransactionId,
    Guid UserId,
    TransactionType Type,
    decimal Amount,
    string Reason,
    DateTime CancelledAt);

public record TransactionFailed(
    Guid TransactionId,
    Guid UserId,
    TransactionType Type,
    decimal Amount,
    string Reason,
    DateTime FailedAt);

public record ItemSold(
    Guid TransactionId,
    Guid SellerId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    DateTime SoldAt);

public record ItemPurchased(
    Guid TransactionId,
    Guid BuyerId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    DateTime PurchasedAt);
