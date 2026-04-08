using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Products.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, GetCategoriesResponse>
    {
        private readonly IProductRepository _productRepository;

        public GetCategoriesHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetCategoriesResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _productRepository.GetCategoriesAsync();
            return new GetCategoriesResponse { Categories = categories };
        }
    }
}
