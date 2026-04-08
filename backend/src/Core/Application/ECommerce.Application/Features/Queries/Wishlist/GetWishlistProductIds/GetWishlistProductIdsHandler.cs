using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlistProductIds
{
    public class GetWishlistProductIdsHandler : IRequestHandler<GetWishlistProductIdsQuery, GetWishlistProductIdsResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistProductIdsHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<GetWishlistProductIdsResponse> Handle(GetWishlistProductIdsQuery request, CancellationToken cancellationToken)
        {
            var ids = await _wishlistRepository.GetProductIdsByUserIdAsync(request.UserId);
            return new GetWishlistProductIdsResponse { ProductIds = ids };
        }
    }
}
