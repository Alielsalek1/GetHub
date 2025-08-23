namespace CatalogService.Domain.models;
public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string SKU { get; set; } = null!;
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }

    // Store attributes as a JSON object instead of separate table
    public Dictionary<string, string>? Attributes { get; set; }
}