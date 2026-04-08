using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlistCount
{
    public class GetWishlistCountHandler : IRequestHandler<GetWishlistCountQuery, GetWishlistCountResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistCountHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<GetWishlistCountResponse> Handle(GetWishlistCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _wishlistRepository.GetCountByUserIdAsync(request.UserId);
            return new GetWishlistCountResponse { Count = count };
        }
    }
}
