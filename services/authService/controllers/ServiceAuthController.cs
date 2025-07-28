using Microsoft.AspNetCore.Mvc;
using authService.Interfaces;
using authService.Dtos;
using SharedKernel;

namespace authService.controllers;

/// <summary>
/// Controller responsible for handling service-to-service authentication.
/// Provides endpoints for issuing authentication tokens for inter-service communication.
/// </summary>
[ApiController]
[Route("service-auth")]
public class ServiceAuthController(IServiceAuthService serviceAuthService) : ControllerBase
{
    private readonly IServiceAuthService _serviceAuthService = serviceAuthService;

    /// <summary>
    /// Issues a service authentication token for inter-service communication.
    /// </summary>
    /// <param name="request">The service token request containing service details</param>
    /// <param name="serviceSecret">The service secret header for authentication</param>
    /// <returns>An API response containing the issued service token</returns>
    /// <response code="200">Service token issued successfully</response>
    /// <response code="401">Unauthorized service request</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("token")]
    public async Task<IActionResult> IssueServiceToken([FromBody] ServiceTokenRequest request, [FromHeader(Name = "X-Service-Secret")] string? serviceSecret)
    {
        var result = await _serviceAuthService.IssueServiceTokenAsync();
        var response = new ApiResponse("Service token issued successfully", 200, new { token = result.Value });
        return StatusCode(200, response);
    }

}
