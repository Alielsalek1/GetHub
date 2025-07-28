namespace userService.Models;
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Bio { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}