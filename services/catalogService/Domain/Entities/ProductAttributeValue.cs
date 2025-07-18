namespace catalogService.Domain.Entities;

public class ProductAttributeValue
{
    public Guid ProductId { get; set; }
    public int AttributeId { get; set; }
    public string Value { get; set; } = null!;

    // Navigation
    public Product Product { get; set; } = null!;
    public ProductAttribute Attribute { get; set; } = null!;
}
