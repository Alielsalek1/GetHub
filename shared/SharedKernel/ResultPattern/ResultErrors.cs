using FluentResults;

namespace SharedKernel;
public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
        Metadata.Add("HttpStatus", 404);
    }
}

public class ConflictError : Error
{
    public ConflictError(string message) : base(message)
    {
        Metadata.Add("HttpStatus", 409);
    }
}

public class ValidationError : Error
{
    public ValidationError(string message) : base(message)
    {
        Metadata.Add("HttpStatus", 400);
    }
}

public class UnauthorizedError : Error
{
    public UnauthorizedError(string message) : base(message)
    {
        Metadata.Add("HttpStatus", 401);
    }
}
