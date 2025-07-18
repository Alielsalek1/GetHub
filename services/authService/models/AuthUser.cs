using authService.enums;

namespace authService.models;

public class AuthUser
{
    public Guid Id { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public string PasswordHash { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiry { get; set; }
    public bool IsRefreshTokenRevoked { get; set; } = false;
    public AuthScheme AuthScheme { get; set; } = AuthScheme.Local;
}