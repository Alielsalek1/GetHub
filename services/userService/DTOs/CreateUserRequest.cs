namespace userService.DTOs;

public class CreateUserRequest
{
    public required string phoneNumber { get; set; } = null!;
    public string? address { get; set; }
}