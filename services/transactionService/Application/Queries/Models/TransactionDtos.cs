using transactionService.Domain.Enums;

namespace transactionService.Application.Queries.Models;

public class TransactionDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; }
    public string TypeName => Type.ToString();
    public TransactionStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName => PaymentMethod.ToString();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Description { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal TotalItemsValue { get; set; }
    public int TotalItemsCount { get; set; }
    public List<TransactionItemDetailsDto> Items { get; set; } = new();
}

public class TransactionItemDetailsDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TransactionSummaryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TransactionType Type { get; set; }
    public string TypeName => Type.ToString();
    public TransactionStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public int TotalItemsCount { get; set; }
}

public class UserTransactionStatsDto
{
    public Guid UserId { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalPurchaseCount { get; set; }
    public int TotalSaleCount { get; set; }
    public decimal AveragePurchaseAmount { get; set; }
    public decimal AverageSaleAmount { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public DateTime? LastSaleDate { get; set; }
    public decimal NetBalance { get; set; } // Sales - Purchases
}
