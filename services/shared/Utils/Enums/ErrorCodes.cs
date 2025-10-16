namespace Shared.Enums;

/// <summary>
/// Standardized error codes for the GetHub microservices architecture.
/// These codes provide consistent error identification across all services.
/// </summary>
public static class ErrorCodes
{
    // === VALIDATION ERRORS ===
    public const string VALIDATION_FAILED = "validation_failed";

    // === USER ERRORS ===
    public const string USER_NOT_FOUND = "user_not_found";
    public const string USER_ALREADY_EXISTS = "user_already_exists";

    // === AUTHENTICATION ERRORS ===
    public const string UNAUTHORIZED = "unauthorized";

    // === AUTHORIZATION ERRORS ===
    public const string FORBIDDEN = "forbidden";


    // === RESOURCE ERRORS ===


    // === CATALOG ERRORS ===


    // === CART ERRORS ===


    // === ORDER ERRORS ===


    // === PAYMENT ERRORS ===


    // === EXTERNAL SERVICE ERRORS ===


    // === SYSTEM ERRORS ===


    // === BUSINESS LOGIC ERRORS ===


    // === NETWORK ERRORS ===


    // === FILE/MEDIA ERRORS ===


    // === GENERIC/UNKNOWN ERRORS ===
    public const string UNEXPECTED_ERROR = "unexpected_error";
}