using FluentResults;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace SharedKernel.Extensions;

/// <summary>
/// Extension methods for converting FluentResults to API responses
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a FluentResults Result&lt;T&gt; to an API-compatible IResult with standardized response format
    /// </summary>
    /// <typeparam name="T">The type of data returned on success</typeparam>
    /// <param name="result">The FluentResults Result to convert</param>
    /// <param name="successStatusCode">HTTP status code to return on success</param>
    /// <param name="successMessage">Optional custom success message</param>
    /// <returns>An IResult containing the standardized API response</returns>
    public static IResult ToApiResult<T>(
        this Result<T> result,
        int successStatusCode,
        string? successMessage = null
    )
    {
        if (result.IsSuccess)
        {
            var value = result.Value;
            var response = new ApiResponse(
                successMessage ?? "Operation successful",
                successStatusCode,
                value
            );

            return Results.Json(response, statusCode: response.statusCode);
        }

        var error = result.Errors.FirstOrDefault();

        int status;
        if (error != null && error.Metadata.TryGetValue("HttpStatus", out var code))
            status = Convert.ToInt32(code);
        else
            status = StatusCodes.Status500InternalServerError;

        return Results.Json(new ApiResponse(
            message: error?.Message ?? "An unexpected error occurred",
            statusCode: status,
            data: result.Errors.Select(e => e.Message).ToArray()
        ), statusCode: status);
    }

    /// <summary>
    /// Converts a FluentResults Result to an API-compatible IResult with standardized response format
    /// </summary>
    /// <param name="result">The FluentResults Result to convert</param>
    /// <param name="successStatusCode">HTTP status code to return on success</param>
    /// <param name="successMessage">Optional custom success message</param>
    /// <returns>An IResult containing the standardized API response</returns>
    public static IResult ToApiResult(
        this Result result,
        int successStatusCode,
        string? successMessage = null
        )
    {
        if (result.IsSuccess)
        {
            var response = new ApiResponse(
                successMessage ?? "Operation successful",
                successStatusCode,
                null
            );

            return Results.Json(response, statusCode: response.statusCode);
        }

        var error = result.Errors.FirstOrDefault();

        int status;
        if (error != null && error.Metadata.TryGetValue("HttpStatus", out var code))
        {
            status = Convert.ToInt32(code);
        }
        else
        {
            status = StatusCodes.Status400BadRequest;
        }

        return Results.Json(new ApiResponse(
            message: error?.Message ?? "An unexpected error occurred",
            statusCode: status,
            data: result.Errors.Select(e => e.Message).ToArray()
        ), statusCode: status);
    }
}
