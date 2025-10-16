using FluentValidation;
using userService.DTOs;
using Shared.Utils;

namespace userService.Validators;

/// <summary>
/// Validator for CreateUserRequest DTO.
/// Validates the phoneNumber field using shared validators.
/// </summary>
/// <remarks>
/// This validator uses shared validators from the Utils library to ensure consistency
/// across the application. PhoneNumber is required, address is optional.
/// </remarks>
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.phoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .SetValidator(new PhoneNumberValidator());
        
        RuleFor(x => x.address)
            .SetValidator(new AddressValidator())
            .When(x => !string.IsNullOrEmpty(x.address));
    }
}