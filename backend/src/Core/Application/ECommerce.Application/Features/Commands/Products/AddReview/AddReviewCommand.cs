using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.Products.AddReview
{
    public class AddReviewCommand : IRequest<AddReviewResponse>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
