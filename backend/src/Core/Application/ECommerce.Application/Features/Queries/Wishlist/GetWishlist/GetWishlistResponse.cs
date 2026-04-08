namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlist
{
    public class GetWishlistResponse
    {
        public IEnumerable<object> Items { get; set; } = Enumerable.Empty<object>();
    }
}
