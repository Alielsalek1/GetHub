namespace CatalogService.Application.DTOs;

public class CategoryTreeResponse
{
    public int id { get; set; }
    public string name { get; set; } = null!;
    public List<CategoryResponse> ancestors { get; set; } = new List<CategoryResponse>();
}
