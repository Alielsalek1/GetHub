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

    // Category
    public const string CATEGORY_NOT_FOUND = "category_not_found";
    public const string CATEGORY_ALREADY_EXISTS = "category_already_exists";
    public const string CATEGORY_CIRCULAR_DEPENDENCY = "category_circular_dependency";
    public const string INVALID_CATEGORY_DELETE = "invalid_category_delete";
    public const string PARENT_CATEGORY_NOT_FOUND = "parent_category_not_found";
    public const string CATEGORY_WITH_SAME_NAME_EXISTS = "category_with_same_name_exists";

    // Product
    public const string PRODUCT_NOT_FOUND = "product_not_found";
    public const string PRODUCT_ALREADY_EXISTS = "product_already_exists";


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