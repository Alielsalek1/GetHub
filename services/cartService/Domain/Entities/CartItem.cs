namespace cartService.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public DateTime AddedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation property
    public Cart Cart { get; private set; } = null!;

    public decimal TotalPrice => UnitPrice * Quantity;

    // Private constructor for EF
    private CartItem() { }

    public CartItem(Guid cartId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty");

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Id = Guid.NewGuid();
        CartId = cartId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        AddedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newUnitPrice)
    {
        if (newUnitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero");

        UnitPrice = newUnitPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
