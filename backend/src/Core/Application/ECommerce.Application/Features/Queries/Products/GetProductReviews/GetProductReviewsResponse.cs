using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Products.GetProductReviews
{
    public class GetProductReviewsResponse
    {
        public IEnumerable<ReviewDto> Reviews { get; set; } = Enumerable.Empty<ReviewDto>();
    }
}
