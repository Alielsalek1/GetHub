namespace CatalogService.Application.DTOs;

public class UpdateCategoryRequest
{
    public string? name { get; set; }
    public int? parentId { get; set; }
}
