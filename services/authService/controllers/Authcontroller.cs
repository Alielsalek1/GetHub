using authService.Dtos;
using authService.DTOs;
using authService.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Annotations;
using SharedKernel.Enums;
using SharedKernel.Extensions;
using Serilog;

namespace authService.controllers;

/// <summary>
/// Controller responsible for handling authentication operations including user registration and login.
/// Provides endpoints for user authentication workflows with comprehensive logging.
/// </summary>
[ApiController]
[Route("auth")]
[Authorize]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly Serilog.ILogger _logger = Log.ForContext<AuthController>();

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="dto">The registration request containing user details</param>
    /// <returns>An API result indicating success or failure of the registration</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid input data or registration failed</response>
    [HttpPost("register")]
    [AuthorizeAuthType(AuthType.Anonymous)]
    public async Task<IResult> Register([FromBody] RegisterRequest dto)
    {
        _logger.Information("User registration attempt for email: {Email}", dto.email);
        
        var result = await authService.RegisterAsync(dto);

        if (result.IsSuccess)
            _logger.Information("User registration successful for email: {Email}", dto.email);
        else
            _logger.Warning("User registration failed for email: {Email}. Errors: {Errors}", 
                dto.email, string.Join(", ", result.Errors.Select(e => e.Message)));

        return result.ToApiResult(
            successMessage: "User registered successfully",
            successStatusCode: 200
        );
    }

    /// <summary>
    /// Authenticates a user with email and password credentials.
    /// </summary>
    /// <param name="dto">The login request containing user credentials</param>
    /// <returns>An API result with authentication token if successful</returns>
    /// <response code="200">Login successful with authentication token</response>
    /// <response code="401">Invalid credentials provided</response>
    /// <response code="400">Invalid input data</response>
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest dto)
    {
        _logger.Information("User login attempt for email: {Email}", dto.email);
        
        var result = await authService.LoginAsync(dto);

        if (result.IsSuccess)
            _logger.Information("User login successful for email: {Email}", dto.email);
        else
            _logger.Warning("User login failed for email: {Email}. Errors: {Errors}", 
                dto.email, string.Join(", ", result.Errors.Select(e => e.Message)));

        return result.ToApiResult(
            successMessage: "Login successful",
            successStatusCode: 200
        );
    }

    // [HttpGet("externallogin")]
    // public async Task<IActionResult> ExternalLogin()
    // {
    //     var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(ExternalLoginCallback)) };
    //     return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    // }

    // [HttpGet("externallogincallback")]
    // public async Task<IActionResult> ExternalLoginCallback()
    // {
    //     var content = await authService.OauthLogin(HttpContext);
    //     var response = new ApiResponse("Login successful", 200, content);
    //     return StatusCode(200, response);
    // }


    // [HttpPost("refresh")]
    // public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequest dto)
    // {
    //     var token = await tokenService.RefreshAsync(dto);
    //     var response = new ApiResponse("Token refreshed successfully", 200, token);
    //     return StatusCode(200, response);
    // }

    // [HttpPost("request-reset-password")]
    // public async Task<IActionResult> RequestPasswordReset([FromBody] ActionRequest dto)
    // {
    //     await authService.RequestPasswordReset(dto);
    //     var response = new ApiResponse("Password reset email sent successfully", 200);
    //     return StatusCode(200, response);
    // }

    // [HttpPost("reset-password")]
    // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
    // {
    //     var user = await authService.ResetPassword(dto);
    //     var response = new ApiResponse("Password reset successfully", 200, user);
    //     return StatusCode(200, response);
    // }

    // [HttpPost("request-activate-email")]
    // public async Task<IActionResult> RequestActivation([FromBody] ActionRequest dto)
    // {
    //     await authService.RequestActivationAsync(dto);
    //     var response = new ApiResponse("Activation Email sent successfully", 200);
    //     return StatusCode(200, response);
    // }

    // [HttpPost("activate-email")]
    // public async Task<IActionResult> Activate([FromQuery] ApplyActivationRequest dto)
    // {
    //     await authService.ActivateUserAsync(dto);
    //     var response = new ApiResponse("Account activated successfully.", 200);
    //     return StatusCode(200, response);
    // }
}