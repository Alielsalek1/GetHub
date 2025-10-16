using FluentValidation;
using userService.DTOs;
using Shared.Utils;

namespace userService.Validators;

/// <summary>
/// Validator for UpdateUserRequest DTO.
/// </summary>
/// <remarks>
/// This validator checks that optional fields meet the required format when provided.
/// All fields are optional for updates, but must be valid if provided.
/// Uses shared validators for consistency across microservices.
/// </remarks>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.bankAccountNumber)
            .SetValidator(new BankAccountValidator())
            .When(x => x.bankAccountNumber != null);

        RuleFor(x => x.bio)
            .SetValidator(new BioValidator())
            .When(x => x.bio != null);

        RuleFor(x => x.phoneNumber)
            .SetValidator(new PhoneNumberValidator())
            .When(x => x.phoneNumber != null);

        RuleFor(x => x.address)
            .SetValidator(new AddressValidator())
            .When(x => x.address != null);

        RuleFor(x => x.profileImageUrl)
            .SetValidator(new UrlValidator())
            .When(x => x.profileImageUrl != null);
    }
}