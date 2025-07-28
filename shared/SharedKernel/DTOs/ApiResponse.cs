namespace SharedKernel;

public class ApiResponse
{
    public string message { get; set; }
    public int statusCode { get; set; }
    public object? data { get; set; }
    public bool success => statusCode >= 200 && statusCode < 300;
    public DateTime timeStamp { get; set; } = DateTime.UtcNow;

    public ApiResponse()
    {
        message = string.Empty;
        statusCode = 0;
        data = null;
    }

    public ApiResponse(string message, int statusCode, object? data)
    {
        this.message = message;
        this.statusCode = statusCode;
        this.data = data;
    }

    public ApiResponse(string message, int statusCode) :
    this(message, statusCode, null)
    {}
}