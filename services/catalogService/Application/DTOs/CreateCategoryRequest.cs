using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs;

public class CreateCategoryRequest
{
    public string Name { get; set; } = null!;
    public int? ParentId { get; set; }
}
