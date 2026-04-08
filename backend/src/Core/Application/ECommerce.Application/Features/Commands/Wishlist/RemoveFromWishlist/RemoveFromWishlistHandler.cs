using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Wishlist.RemoveFromWishlist
{
    public class RemoveFromWishlistHandler : IRequestHandler<RemoveFromWishlistCommand, RemoveFromWishlistResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public RemoveFromWishlistHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<RemoveFromWishlistResponse> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            var item = await _wishlistRepository.GetItemAsync(request.UserId, request.ProductId);
            if (item == null)
            {
                return new RemoveFromWishlistResponse { IsSuccess = false, Message = "Ürün favorilerizde bulunamadı." };
            }

            await _wishlistRepository.RemoveAsync(item);
            return new RemoveFromWishlistResponse { IsSuccess = true, Message = "Favorilerden kaldırıldı." };
        }
    }
}
