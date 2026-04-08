using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Products.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return new DeleteProductResponse { IsSuccess = false, Message = "Ürün bulunamadı." };
            }

            await _productRepository.DeleteAsync(product);

            return new DeleteProductResponse { IsSuccess = true, Message = "Ürün başarıyla silindi." };
        }
    }
}
