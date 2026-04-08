using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Products.GetProducts
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, GetProductsResponse>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(request.Search, request.Category);
            return new GetProductsResponse { Products = products };
        }
    }
}
