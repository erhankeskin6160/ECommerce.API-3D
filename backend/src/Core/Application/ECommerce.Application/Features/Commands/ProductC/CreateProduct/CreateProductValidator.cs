using FluentValidation;

namespace ECommerce.Application.Features.Commands.ProductC.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
        }
    }
}
