using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public int? CategoryId { get; set; }
}
