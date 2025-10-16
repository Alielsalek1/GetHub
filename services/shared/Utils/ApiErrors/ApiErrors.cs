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