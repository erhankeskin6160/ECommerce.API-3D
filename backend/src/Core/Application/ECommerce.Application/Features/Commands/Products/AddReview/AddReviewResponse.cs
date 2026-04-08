using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.Products.AddReview
{
    public class AddReviewResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public ReviewDto? Review { get; set; }
    }
}
