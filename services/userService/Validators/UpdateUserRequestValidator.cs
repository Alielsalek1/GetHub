using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using URLshortner.Dtos;
using URLshortner.Utils;
using userService.DTOs;

namespace URLshortner.Dtos.Validators;

/// <summary>
/// Validator for UpdateUserRequest DTO.
/// </summary>
/// <remarks>
/// This validator checks that the name field is not empty and meets the required format.
/// </remarks>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.name).SetValidator(new UsernameValidator());
    }
}