using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Commands.ProductC.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                Category = request.Category,
                ImageUrl = request.ImageUrl
            };

            var created = await _productRepository.CreateAsync(product);
            return new CreateProductResponse
            {
                IsSuccess = true,
                Message = "Ürün başarıyla oluşturuldu.",
                ProductId = created.Id
            };
        }
    }
}
