namespace CatalogService.Application.DTOs;

public class UpdateCategoryRequest
{
    public string? Name { get; set; }
    public int? ParentId { get; set; }
}
