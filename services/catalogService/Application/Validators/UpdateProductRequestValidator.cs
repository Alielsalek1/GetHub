using FluentValidation;
using CatalogService.Application.DTOs;
using Shared.Utils;

namespace CatalogService.Application.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name cannot be empty when provided")
                .MaximumLength(300).WithMessage("Product name must be at most 300 characters");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must be at most 2000 characters");
        });

        When(x => x.Brand != null, () =>
        {
            RuleFor(x => x.Brand)
                .MaximumLength(200).WithMessage("Brand must be at most 200 characters");
        });

        When(x => x.CategoryId.HasValue, () =>
        {
            RuleFor(x => x.CategoryId!.Value).SetValidator(new IdValidator());
        });
    }
}
