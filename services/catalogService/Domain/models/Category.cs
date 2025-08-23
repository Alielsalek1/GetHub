namespace CatalogService.Domain.models;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public int? ParentId { get; set; }
    public Category? ParentCategory { get; set; }

    public ICollection<Category>? Subcategories { get; set; }
    public ICollection<Product>? Products { get; set; }
}
