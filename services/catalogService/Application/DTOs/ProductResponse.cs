namespace CatalogService.Application.DTOs;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
