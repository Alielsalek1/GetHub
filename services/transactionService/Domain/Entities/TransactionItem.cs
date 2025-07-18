namespace transactionService.Domain.Entities;

public class TransactionItem
{
    public Guid Id { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public Transaction Transaction { get; private set; } = null!;

    // Computed property
    public decimal TotalPrice => UnitPrice * Quantity;

    // Private constructor for EF
    private TransactionItem() { }

    public TransactionItem(
        Guid transactionId,
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        Id = Guid.NewGuid();
        TransactionId = transactionId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Quantity = newQuantity;
    }

    public void UpdatePrice(decimal newUnitPrice)
    {
        if (newUnitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        UnitPrice = newUnitPrice;
    }
}
