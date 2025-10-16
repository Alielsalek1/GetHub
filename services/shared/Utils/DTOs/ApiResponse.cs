namespace Shared;

public class ApiResponse
{
    public required string message { get; set; }
    public object? data { get; set; }
    public string? errorCode { get; set; }
    public List<string>? errors { get; set; }
    public DateTime timeStamp { get; set; } = DateTime.UtcNow;
}