using transactionService.Domain.Enums;

namespace transactionService.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Description { get; private set; }
    public string? ReferenceNumber { get; private set; }
    
    // Navigation properties
    private readonly List<TransactionItem> _items = new();
    public IReadOnlyCollection<TransactionItem> Items => _items.AsReadOnly();

    // Computed properties
    public decimal TotalItemsValue => _items.Sum(item => item.TotalPrice);
    public int TotalItemsCount => _items.Sum(item => item.Quantity);

    // Private constructor for EF
    private Transaction() { }

    public Transaction(
        Guid userId,
        TransactionType type,
        decimal amount,
        PaymentMethod paymentMethod,
        string? description = null,
        string currency = "USD")
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        Id = Guid.NewGuid();
        UserId = userId;
        Type = type;
        Status = TransactionStatus.Pending;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        Description = description;
        ReferenceNumber = GenerateReferenceNumber();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending transaction");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        var item = new TransactionItem(Id, productId, productName, quantity, unitPrice);
        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TransactionStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == TransactionStatus.Completed)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void Complete()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException("Only pending transactions can be completed");

        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed transaction");

        Status = TransactionStatus.Cancelled;
        Description = reason ?? Description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail(string? reason = null)
    {
        Status = TransactionStatus.Failed;
        Description = reason ?? Description;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateReferenceNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"TXN-{timestamp}-{random}";
    }
}
