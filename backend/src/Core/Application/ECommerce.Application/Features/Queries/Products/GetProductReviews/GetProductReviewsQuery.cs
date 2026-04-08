using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Products.GetProductReviews
{
    public record GetProductReviewsQuery(int ProductId) : IRequest<GetProductReviewsResponse>;
}
