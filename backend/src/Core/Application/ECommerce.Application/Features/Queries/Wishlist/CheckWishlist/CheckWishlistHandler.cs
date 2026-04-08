using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Wishlist.CheckWishlist
{
    public class CheckWishlistHandler : IRequestHandler<CheckWishlistQuery, CheckWishlistResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public CheckWishlistHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<CheckWishlistResponse> Handle(CheckWishlistQuery request, CancellationToken cancellationToken)
        {
            var exists = await _wishlistRepository.ExistsAsync(request.UserId, request.ProductId);
            return new CheckWishlistResponse { IsFavorited = exists };
        }
    }
}
