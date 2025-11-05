using FluentResults;
using Shared.Enums;

namespace Shared;

public class UserNotFoundError : Error
{
    public UserNotFoundError() : base()
    {
        Metadata.Add("httpStatus", 404);
        Metadata.Add("message", "User not found.");
        Metadata.Add("errorCode", ErrorCodes.USER_NOT_FOUND);
    }
}

public class UserAlreadyExistsError : Error
{
    public UserAlreadyExistsError() : base()
    {
        Metadata.Add("httpStatus", 409);
        Metadata.Add("message", "User already exists.");
        Metadata.Add("errorCode", ErrorCodes.USER_ALREADY_EXISTS);
    }
}

public class ValidationError : Error
{
    public ValidationError() : base()
    {
        Metadata.Add("httpStatus", 400);
        Metadata.Add("message", "Validation failed.");
        Metadata.Add("errorCode", ErrorCodes.VALIDATION_FAILED);
    }
}

public class CategoryNotFoundError : Error
{
    public CategoryNotFoundError() : base()
    {
        Metadata.Add("httpStatus", 404);
        Metadata.Add("message", "Category not found.");
        Metadata.Add("errorCode", ErrorCodes.CATEGORY_NOT_FOUND);
    }
}

public class CategoryAlreadyExistsError : Error
{
    public CategoryAlreadyExistsError() : base()
    {
        Metadata.Add("httpStatus", 409);
        Metadata.Add("message", "Category already exists.");
        Metadata.Add("errorCode", ErrorCodes.CATEGORY_ALREADY_EXISTS);
    }
}

public class CategoryCircularDependencyUpdateError : Error
{
    public CategoryCircularDependencyUpdateError() : base()
    {
        Metadata.Add("httpStatus", 400);
        Metadata.Add("message", "Invalid category update due to circular dependency.");
        Metadata.Add("errorCode", ErrorCodes.CATEGORY_CIRCULAR_DEPENDENCY);
    }
}

// category with same name exists for update
public class CategoryWithSameNameAlreadyExistsError : Error
{
    public CategoryWithSameNameAlreadyExistsError() : base()
    {
        Metadata.Add("httpStatus", 409);
        Metadata.Add("message", "Category with the same name already exists.");
        Metadata.Add("errorCode", ErrorCodes.CATEGORY_WITH_SAME_NAME_EXISTS);
    }
}

public class InvalidCategoryDeleteError : Error
{
    public InvalidCategoryDeleteError() : base()
    {
        Metadata.Add("httpStatus", 400);
        Metadata.Add("message", "Cannot delete category with existing child categories.");
        Metadata.Add("errorCode", ErrorCodes.INVALID_CATEGORY_DELETE);
    }
}

public class ParentCategoryNotFoundError : Error
{
    public ParentCategoryNotFoundError() : base()
    {
        Metadata.Add("httpStatus", 404);
        Metadata.Add("message", "Parent category not found.");
        Metadata.Add("errorCode", ErrorCodes.PARENT_CATEGORY_NOT_FOUND);
    }
}

public class ProductAlreadyExistsError : Error
{
    public ProductAlreadyExistsError() : base()
    {
        Metadata.Add("httpStatus", 409);
        Metadata.Add("message", "Product already exists.");
        Metadata.Add("errorCode", ErrorCodes.PRODUCT_ALREADY_EXISTS);
    }
}

public class ProductNotFoundError : Error
{
    public ProductNotFoundError() : base()
    {
        Metadata.Add("httpStatus", 404);
        Metadata.Add("message", "Product not found.");
        Metadata.Add("errorCode", ErrorCodes.PRODUCT_NOT_FOUND);
    }
}

public class UnauthorizedError : Error
{
    public UnauthorizedError() : base()
    {
        Metadata.Add("httpStatus", 401);
        Metadata.Add("message", "Authorization required.");
        Metadata.Add("errorCode", ErrorCodes.UNAUTHORIZED);
    }
}

public class ForbiddenError : Error
{
    public ForbiddenError() : base()
    {
        Metadata.Add("httpStatus", 403);
        Metadata.Add("message", "Access forbidden.");
        Metadata.Add("errorCode", ErrorCodes.FORBIDDEN);
    }
}