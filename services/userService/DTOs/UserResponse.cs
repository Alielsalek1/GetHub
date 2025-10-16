namespace userService.DTOs;

public class UserResponse
{
    public Guid id { get; set; }
    public string phoneNumber { get; set; } = null!;
    public string? address { get; set; }
    public string? bio { get; set; }
    public string? profileImageUrl { get; set; }
    public string? bankAccountNumber { get; set; }
}