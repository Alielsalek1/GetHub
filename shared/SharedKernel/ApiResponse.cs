namespace SharedKernel;

public class ApiResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public object? Data { get; set; }

    public ApiResponse(string message, int statusCode, object? data)
    {
        Message = message;
        StatusCode = statusCode;
        Data = data;
    }

    public ApiResponse(string message, int statusCode) :
    this(message, statusCode, null)
    {}
}