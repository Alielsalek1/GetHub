using FluentValidation;
using CatalogService.Application.DTOs;
using Shared.Utils;

namespace CatalogService.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(300).WithMessage("Product name must be at most 300 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must be at most 2000 characters");

        RuleFor(x => x.Brand)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Brand))
            .WithMessage("Brand must be at most 200 characters");

        RuleFor(x => x.CategoryId).SetValidator(new IdValidator());
    }
}
