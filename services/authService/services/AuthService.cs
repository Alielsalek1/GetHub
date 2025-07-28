using AutoMapper;
using authService.enums;
using Microsoft.AspNetCore.Http;
using authService.Interfaces;
using authService.Dtos;
using authService.models;
using MassTransit;
using FluentResults;
using SharedKernel;
using SharedKernel.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using authService.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace authService.Services;

public class AuthService(
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    ILogger<AuthService> logger,
    IAuthUserRepository userRepository
    ) : IAuthService
{
    private readonly HttpClient usersClient = httpClientFactory.CreateClient("users");

    public async Task<Result> RegisterAsync(RegisterRequest dto)
    {
        var createReq = new
        {
            dto.username,
            dto.email
        };

        var result = await new ServiceAuthService(configuration).IssueServiceTokenAsync();
        var token = result.Value;

        var req = new HttpRequestMessage(HttpMethod.Post, "users")
        {
            Content = JsonContent.Create(createReq)
        };

        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var resp = await usersClient.SendAsync(req);
        
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadFromJsonAsync<ApiResponse>();

            if (error == null || error.data == null)
                return Result.Fail(new CustomError(resp.Content.ToString() ?? "failed", (int)resp.StatusCode));
                
            return Result.Fail(new CustomError(error.message, (int)resp.StatusCode));
        }

        // Get the ApiResponse envelope from the UserService
        var apiResponse = await resp.Content.ReadFromJsonAsync<ApiResponse>();
        if (apiResponse?.data == null)
        {
            logger.LogError("No data returned from UserService");
            return Result.Fail(new CustomError("No user data returned from UserService", 500));
        }

        var userDataJson = (JsonElement)apiResponse.data;
        var userId = userDataJson.GetProperty("id").GetGuid();

        var newUser = new AuthUser
        {
            Id = userId,
            Email = dto.email,
            IsEmailVerified = false,
            PasswordHash = PasswordService.Encode(dto.password),
            RefreshToken = Guid.NewGuid(),
            RefreshTokenExpiry = DateTime.Now.AddDays(10),
            IsRefreshTokenRevoked = false,
            AuthScheme = AuthScheme.Local
        };

        await userRepository.CreateAsync(newUser);

        return Result.Ok();
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest dto, AuthScheme authScheme)
    {
        var curUser = await userRepository.GetByEmailAsync(dto.email);

        if (curUser == null ||
        !PasswordService.Verify(dto.password, curUser.PasswordHash) ||
        curUser.AuthScheme != authScheme)
        {
            return Result.Fail(new InvalidCredentialsError("Invalid Credentials"));
        }

        var refreshToken = curUser.RefreshToken;

        // Renew the refresh token
        curUser.RefreshTokenExpiry = DateTime.Now.AddDays(10);
        await userRepository.UpdateAsync(curUser);

        // access token logic
        var externalSecret = configuration["Jwt:External:Secret"] ?? throw new InvalidOperationException("External JWT secret is not configured.");
        var externalIssuer = configuration["Jwt:External:Issuer"] ?? throw new InvalidOperationException("External JWT issuer is not configured.");
        var externalAudience = configuration["Jwt:External:Audience"] ?? throw new InvalidOperationException("External JWT audience is not configured.");

        var claims = new List<Claim>
        {
            new("user_id", curUser.Id.ToString()),
            new("email", curUser.Email)
        };
        
        var accessToken = JwtTokenService.CreateJwtToken(claims, externalSecret, externalIssuer, externalAudience);

        return new LoginResponse
        {
            accessToken = accessToken,
            refreshToken = refreshToken.ToString(),
            userId = curUser.Id,
            email = curUser.Email
        };
    }

    // public async Task RequestPasswordReset(ActionRequest dto)
    // {
    //     var user = await userRepository.GetByEmailAsync(dto.email);
    //     if (user == null)
    //         throw new NotFoundException("Not found.");
    //     if (user.AuthScheme != AuthScheme.UrlHub)
    //         throw new InvalidInputException();

    //     var token = Guid.NewGuid();

    //     string key = $"reset_token:{user.Id}:{token}";
    //     await ActionTokenRepository.AddAsync(key, token.ToString());

    //     string resetLink = $"{configuration["frontend"]}/reset-password?token={token}&email={user.Email}";
    //     string subject = "Reset Your Password";
    //     string body = $"<h1>Reset Your Password</h1>" +
    //         $"<p>To reset your password, please click the link below:</p>" +
    //         $"<a href='{resetLink}'>Reset Password</a>";

    //     await emailService.SendEmailAsync(user.Email, subject, body);
    // }

    // public async Task<UserResponse> ResetPassword(ResetPasswordRequest dto)
    // {
    //     var user = await userRepository.GetByEmailAsync(dto.email);
    //     if (user == null)
    //         throw new NotFoundException("User not found.");

    //     string key = $"reset_token:{user.Id}:{dto.token}";
    //     var resetToken = await ActionTokenRepository.GetToken(key);

    //     if (string.IsNullOrEmpty(resetToken))
    //         throw new InvalidInputException("Invalid reset token.");

    //     return await userService.UpdateUserAsync(user.Id, new UpdateUserRequest
    //     {
    //         username = user.Username,
    //         password = dto.newPassword
    //     });
    // }

    // public async Task RequestActivationAsync(ActionRequest dto)
    // {
    //     var user = await userRepository.GetByEmailAsync(dto.email);
    //     if (user == null)
    //         throw new NotFoundException();
    //     if (user.IsEmailVerified)
    //         throw new AlreadyVerifiedException();

    //     var token = Guid.NewGuid();

    //     var key = $"activation_token:{user.Id}:{token}";
    //     await ActionTokenRepository.AddAsync(key, token.ToString());

    //     string activationLink =
    //         $"{configuration["server"]}/api/auth/activate-email" +
    //         $"?token={token}&email={user.Email}";

    //     string subject = "Activate Your Account";
    //     string body = $"<h1>Welcome to URL Hub!</h1>" +
    //         $"<p>Please activate your account by clicking <a href='{activationLink}'>here</a>.</p>";

    //     await emailService.SendEmailAsync(user.Email, subject, body);
    // }

    // public async Task ActivateUserAsync(ApplyActivationRequest dto)
    // {
    //     var user = await userRepository.GetByEmailAsync(dto.email);
    //     if (user == null)
    //         throw new NotFoundException("no user found with this Email");
    //     if (user.IsEmailVerified)
    //         throw new AlreadyVerifiedException("user is already verified");
    //     if (user.AuthScheme != AuthScheme.UrlHub)
    //         throw new InvalidInputException("Cannot activate users authenticated via external providers.");

    //     var key = $"activation_token:{user.Id}:{dto.token}";
    //     var activationToken = await ActionTokenRepository.GetToken(key);
    //     if (string.IsNullOrEmpty(activationToken))
    //     {
    //         throw new InvalidInputException("Not a valid ActionToken");
    //     }

    //     user.IsEmailVerified = true;
    //     await userRepository.UpdateAsync(user);
    // }

    // public async Task<AuthResponse> OauthLogin(HttpContext context)
    // {
    //     // Retrieve the external login information after the user has authenticated with Google.
    //     var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //     if (!result.Succeeded)
    //     {
    //         throw new GoogleAuthFailedException("External authentication failed.");
    //     }

    //     // user Data
    //     var externalPrincipal = result.Principal;
    //     string email = externalPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? "";
    //     string name = externalPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? "";

    //     if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
    //     {
    //         throw new InvalidInputException("Invalid external login information.");
    //     }

    //     var user = await userRepository.GetByEmailAsync(email);

    //     if (user == null)
    //     {
    //         // registeration
    //         var passwordHasher = new PasswordHasher<string>(); // for storing hashed passwords in the db
    //         user = new User
    //         {
    //             Username = name,
    //             Email = email,
    //             IsEmailVerified = true,
    //             AuthScheme = AuthScheme.Google,
    //             Password = passwordHasher.HashPassword(null, "0")
    //         };
    //         await userRepository.AddAsync(user);
    //         await tokenService.GenerateRefreshTokenAsync(user);
    //     }

    //     return await this.LoginAsync(new LoginRequest
    //     {
    //         username = user.Username,
    //         password = "0"
    //     },
    //     AuthScheme.Google);
    // }
}