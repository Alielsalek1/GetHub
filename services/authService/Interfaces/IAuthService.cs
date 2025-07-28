using authService.Dtos;
using authService.DTOs;
using authService.enums;
using FluentResults;
using Microsoft.AspNetCore.Http;


namespace authService.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequest dto);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest dto, AuthScheme authScheme = AuthScheme.Local);
    // Task RequestActivationAsync(ActionRequest dto);
    // Task ActivateUserAsync(ApplyActivationRequest dto);
    // Task<AuthResponse> OauthLogin(HttpContext context);
    // Task RequestPasswordReset(ActionRequest dto);
    // Task<UserResponse> ResetPassword(ResetPasswordRequest dto);
}