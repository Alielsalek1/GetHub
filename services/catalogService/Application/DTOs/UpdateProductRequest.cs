namespace catalogService.Application.DTOs;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsActive { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}
