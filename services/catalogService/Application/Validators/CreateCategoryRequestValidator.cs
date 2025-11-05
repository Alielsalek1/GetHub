using FluentValidation;
using CatalogService.Application.DTOs;
using Shared.Utils;

namespace CatalogService.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(200).WithMessage("Category name must be at most 200 characters");

        When(x => x.parentId.HasValue, () =>
        {
            RuleFor(x => x.parentId!.Value).SetValidator(new IdValidator());
        });
    }
}
