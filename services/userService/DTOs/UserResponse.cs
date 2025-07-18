namespace userService.DTOs;

public class UserResponse
{
    public Guid id { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
    public string? bio { get; set; }
    public string? profileImageUrl { get; set; }
}