using authService.enums;
using MongoDB.Bson.Serialization.Attributes;

namespace authService.models;

public class AuthUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public bool IsEmailVerified { get; set; } = false;
    public string PasswordHash { get; set; } = null!;
    public Guid RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public bool IsRefreshTokenRevoked { get; set; } = false;
    public AuthScheme AuthScheme { get; set; } = AuthScheme.Local;
}