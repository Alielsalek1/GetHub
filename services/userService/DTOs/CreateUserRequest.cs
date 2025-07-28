namespace userService.DTOs;

public class CreateUserRequest
{
    public string username { get; set; } = null!;
    public string email { get; set; } = null!;
}