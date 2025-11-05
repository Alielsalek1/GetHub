using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs;

public class CreateCategoryRequest
{
    public string name { get; set; } = null!;
    public int? parentId { get; set; }
}
