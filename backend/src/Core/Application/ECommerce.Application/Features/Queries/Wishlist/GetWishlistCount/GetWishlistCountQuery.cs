using MediatR;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlistCount
{
    public record GetWishlistCountQuery(string UserId) : IRequest<GetWishlistCountResponse>;

    public class GetWishlistCountResponse
    {
        public int Count { get; set; }
    }
}
