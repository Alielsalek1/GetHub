using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using URLshortner.Dtos;
using URLshortner.Utils;
using userService.DTOs;

namespace URLshortner.Dtos.Validators;

/// <summary>
/// Validator for CreateUserRequest DTO.
/// Validates the username and email fields.
/// </summary>
/// <remarks>
/// This validator checks that the username is not empty and meets the required format,
/// and that the email is not empty and is a valid email address.
/// </remarks>
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.username).SetValidator(new UsernameValidator());
        RuleFor(x => x.email).SetValidator(new EmailValidator());
    }
}