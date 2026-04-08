using MediatR;

namespace ECommerce.Application.Features.Queries.Wishlist.CheckWishlist
{
    public record CheckWishlistQuery(string UserId, int ProductId) : IRequest<CheckWishlistResponse>;

    public class CheckWishlistResponse
    {
        public bool IsFavorited { get; set; }
    }
}
