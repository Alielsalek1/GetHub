using FluentValidation;
using System.Text.RegularExpressions;

namespace Shared.Utils;

public class IdValidator : AbstractValidator<int>
{
    public IdValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class GuidValidator : AbstractValidator<Guid>
{
    public GuidValidator()
    {
        RuleFor(x => x)
            .NotEqual(Guid.Empty).WithMessage("GUID cannot be empty.");
    }
}

public class PageSizeValidator : AbstractValidator<int>
{
    public PageSizeValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Page Size must be greater than 0.")
            .LessThan(50).WithMessage("Page Size must be smaller than 50.");
    }
}

public class PageNumberValidator : AbstractValidator<int>
{
    public PageNumberValidator()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
    }
}

public class PhoneNumberValidator : AbstractValidator<string?>
{
    public PhoneNumberValidator()
    {
        RuleFor(x => x)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number must be a valid format (e.g., +1234567890).");
    }
}

public class BankAccountValidator : AbstractValidator<string?>
{
    public BankAccountValidator()
    {
        RuleFor(x => x)
            .Matches(@"^\d{10,20}$").WithMessage("Bank account number must be 10-20 digits.");
    }
}

public class BioValidator : AbstractValidator<string?>
{
    public BioValidator()
    {
        RuleFor(x => x)
            .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.");
    }
}

public class AddressValidator : AbstractValidator<string?>
{
    public AddressValidator()
    {
        RuleFor(x => x)
            .MinimumLength(10).WithMessage("Address must be at least 10 characters long.");
    }
}

public class UrlValidator : AbstractValidator<string?>
{
    public UrlValidator()
    {
        RuleFor(x => x)
            .Must(BeAValidUrl).WithMessage("URL must be a valid HTTP or HTTPS URL.");
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}