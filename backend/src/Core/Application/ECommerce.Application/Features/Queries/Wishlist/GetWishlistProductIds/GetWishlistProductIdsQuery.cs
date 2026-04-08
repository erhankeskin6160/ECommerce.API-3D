using MediatR;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlistProductIds
{
    public record GetWishlistProductIdsQuery(string UserId) : IRequest<GetWishlistProductIdsResponse>;

    public class GetWishlistProductIdsResponse
    {
        public IEnumerable<int> ProductIds { get; set; } = Enumerable.Empty<int>();
    }
}
