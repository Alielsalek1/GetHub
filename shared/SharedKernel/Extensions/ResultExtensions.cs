using FluentResults;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace SharedKernel.Extensions;

public static class ResultExtensions
{
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

            return Results.Json(response, statusCode: response.StatusCode);
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

            return Results.Json(response, statusCode: response.StatusCode);
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
