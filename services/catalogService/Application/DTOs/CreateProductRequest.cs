using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public int CategoryId { get; set; }
}
