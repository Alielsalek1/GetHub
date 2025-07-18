using authService.Dtos;
using authService.enums;
using Microsoft.AspNetCore.Http;


namespace authService.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest dto);
    // Task<AuthResponse> LoginAsync(LoginRequest dto, AuthScheme authScheme = AuthScheme.Local);
    // Task RequestActivationAsync(ActionRequest dto);
    // Task ActivateUserAsync(ApplyActivationRequest dto);
    // Task<AuthResponse> OauthLogin(HttpContext context);
    // Task RequestPasswordReset(ActionRequest dto);
    // Task<UserResponse> ResetPassword(ResetPasswordRequest dto);
}