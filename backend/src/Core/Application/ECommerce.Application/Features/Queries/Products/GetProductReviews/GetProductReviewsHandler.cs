using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Products.GetProductReviews
{
    public class GetProductReviewsHandler : IRequestHandler<GetProductReviewsQuery, GetProductReviewsResponse>
    {
        private readonly IProductRepository _productRepository;

        public GetProductReviewsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductReviewsResponse> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _productRepository.GetReviewsAsync(request.ProductId);
            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? "Anonim",
                Rating = r.Rating,
                Comment = r.Comment,
                ImageUrl = r.ImageUrl,
                CreatedAt = r.CreatedAt
            });

            return new GetProductReviewsResponse { Reviews = reviewDtos };
        }
    }
}
