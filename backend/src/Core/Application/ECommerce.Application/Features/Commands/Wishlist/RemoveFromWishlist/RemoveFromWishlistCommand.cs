using MediatR;

namespace ECommerce.Application.Features.Commands.Wishlist.RemoveFromWishlist
{
    public class RemoveFromWishlistCommand : IRequest<RemoveFromWishlistResponse>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class RemoveFromWishlistResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
