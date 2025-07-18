namespace catalogService.Application.DTOs;

public class CreateAttributeRequest
{
    public string Name { get; set; } = null!;
    public string DataType { get; set; } = "string";
    public bool IsRequired { get; set; } = false;
}
