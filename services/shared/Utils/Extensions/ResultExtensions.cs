using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Shared.Extensions;

/// <summary>
/// Extension methods for converting FluentResults to API responses following SRP principle
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a successful FluentResults Result&lt;T&gt; to an API success response
    /// </summary>
    /// <typeparam name="T">The type of data returned on success</typeparam>
    /// <param name="result">The successful FluentResults Result to convert</param>
    /// <param name="successStatusCode">HTTP status code to return on success (default: 200)</param>
    /// <param name="successMessage">Optional custom success message</param>
    /// <returns>An IActionResult containing the standardized success API response</returns>
    public static IActionResult ToSuccessApiResult<T>(
        this Result<T> result,
        int successStatusCode = 200,
        string? successMessage = null
    )
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("Cannot convert failed result to success response");
        return CreateSuccessResponse(result.Value, successStatusCode, successMessage);
    }

    /// <summary>
    /// Converts a successful FluentResults Result to an API success response
    /// </summary>
    /// <param name="result">The successful FluentResults Result to convert</param>
    /// <param name="successStatusCode">HTTP status code to return on success (default: 200)</param>
    /// <param name="successMessage">Optional custom success message</param>
    /// <returns>An IActionResult containing the standardized success API response</returns>
    public static IActionResult ToSuccessApiResult(
        this Result result,
        int successStatusCode = 200,
        string? successMessage = null
    )
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("Cannot convert failed result to success response");
        return CreateSuccessResponse(null, successStatusCode, successMessage);
    }

    /// <summary>
    /// Converts a failed FluentResults Result&lt;T&gt; to an API error response
    /// </summary>
    /// <typeparam name="T">The type of data that would be returned on success</typeparam>
    /// <param name="result">The failed FluentResults Result to convert</param>
    /// <param name="additionalErrors">Additional error messages to include</param>
    /// <returns>An IActionResult containing the standardized error API response</returns>
    public static IActionResult ToErrorApiResult<T>(
        this Result<T> result,
        List<string>? additionalErrors = null
    )
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert successful result to error response.");
        return CreateErrorResponse(result.Errors, additionalErrors);
    }

    /// <summary>
    /// Converts a failed FluentResults Result to an API error response
    /// </summary>
    /// <param name="result">The failed FluentResults Result to convert</param>
    /// <param name="additionalErrors">Additional error messages to include</param>
    /// <returns>An IActionResult containing the standardized error API response</returns>
    public static IActionResult ToErrorApiResult(
        this Result result,
        List<string>? additionalErrors = null
    )
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert successful result to error response");
        return CreateErrorResponse(result.Errors, additionalErrors);
    }

    /// <summary>
    /// Internal method that creates success API responses
    /// </summary>
    private static IActionResult CreateSuccessResponse(
        object? data,
        int statusCode,
        string? message
    )
    {
        // return an empty status result rather than an ApiResponse wrapper.
        if (statusCode == StatusCodes.Status201Created || statusCode == StatusCodes.Status204NoContent)
            return new StatusCodeResult(statusCode);

        var response = new ApiResponse
        {
            message = message ?? "Operation successful",
            data = data,
        };
        return new JsonResult(response) { StatusCode = statusCode };
    }

    /// <summary>
    /// Internal method that creates error API responses from FluentResults errors
    /// </summary>
    private static IActionResult CreateErrorResponse(
        IReadOnlyList<IError> errors,
        List<string>? additionalErrors
    )
    {
        var primaryError = errors.FirstOrDefault();
        
        var errorMessage = primaryError?.Metadata.TryGetValue("message", out var msgObj) == true 
            ? msgObj.ToString() 
            : null;
        var statusCode = primaryError?.Metadata.TryGetValue("httpStatus", out var statusObj) == true 
            ? Convert.ToInt32(statusObj)
            : StatusCodes.Status500InternalServerError;
        var errorCode = primaryError?.Metadata.TryGetValue("errorCode", out var codeObj) == true 
            ? codeObj.ToString() 
            : null;

        var errorResponse = new ApiResponse
        {
            message = errorMessage ?? "An unexpected error occurred",
            errorCode = errorCode ?? "ERR",
            errors = additionalErrors
        };
        return new JsonResult(errorResponse) { StatusCode = statusCode };
    }
}