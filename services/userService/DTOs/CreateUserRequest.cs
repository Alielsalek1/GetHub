namespace userService.DTOs;

public class CreateUserRequest
{
    public Guid userId { get; set; }
    public string name { get; set; } = null!;
    public string email { get; set; } = null!;
}