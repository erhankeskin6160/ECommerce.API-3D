using MediatR;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlist
{
    public record GetWishlistQuery(string UserId) : IRequest<GetWishlistResponse>;
}

