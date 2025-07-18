namespace userService.DTOs;

public class UpdateUserRequest
{
    public string? name { get; set; }
    public string? bio { get; set; }
    public string? profileImageUrl { get; set; }
}