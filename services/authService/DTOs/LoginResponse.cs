namespace authService.DTOs;

public class LoginResponse
{
    public Guid userId { get; set; }
    public string email { get; set; } = null!;
    public string accessToken { get; set; } = null!;
    public string refreshToken { get; set; } = null!;
}