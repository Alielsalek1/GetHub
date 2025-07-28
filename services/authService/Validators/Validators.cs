using FluentValidation;
using System.Text.RegularExpressions;

namespace authService.Validators;

/// <summary>
/// Validator for integer ID values, ensuring they are positive.
/// </summary>
public class IdValidator : AbstractValidator<int>
{
    /// <summary>
    /// Initializes a new instance of the IdValidator class with validation rules.
    /// </summary>
    public IdValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

/// <summary>
/// Validator for username strings, ensuring they meet format and length requirements.
/// Validates that usernames are alphanumeric and have appropriate length.
/// </summary>
public class UsernameValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the UsernameValidator class with validation rules.
    /// </summary>
    public UsernameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .Matches("^[a-zA-Z0-9]*$").WithMessage("Username must be alphanumeric.");
    }
}

/// <summary>
/// Validator for password strings, ensuring they meet security requirements.
/// Validates password complexity including uppercase, lowercase, numbers, and special characters.
/// </summary>
public class PasswordValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the PasswordValidator class with validation rules.
    /// </summary>
    public PasswordValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}

/// <summary>
/// Validator for email address strings, ensuring they are properly formatted.
/// Validates that email addresses meet the standard email format requirements.
/// </summary>
public class EmailValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the EmailValidator class with validation rules.
    /// </summary>
    public EmailValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}