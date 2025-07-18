namespace catalogService.Domain.Entities;

public class ProductAttribute
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DataType { get; set; } = "string"; // string, int, decimal, bool
    public bool IsRequired { get; set; } = false;

    // Navigation
    public ICollection<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();
}
