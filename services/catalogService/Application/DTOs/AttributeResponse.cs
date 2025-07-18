namespace catalogService.Application.DTOs;

public class AttributeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DataType { get; set; } = null!;
    public bool IsRequired { get; set; }
}
