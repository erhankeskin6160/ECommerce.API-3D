using MediatR;

namespace ECommerce.Application.Features.Commands.ProductC.CreateProduct
{
    public class CreateProductRequest : IRequest<CreateProductResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
    }
}
