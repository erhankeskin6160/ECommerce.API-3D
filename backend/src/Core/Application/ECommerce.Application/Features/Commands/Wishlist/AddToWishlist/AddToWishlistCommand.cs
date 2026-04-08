using MediatR;

namespace ECommerce.Application.Features.Commands.Wishlist.AddToWishlist
{
    public class AddToWishlistCommand : IRequest<AddToWishlistResponse>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class AddToWishlistResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
