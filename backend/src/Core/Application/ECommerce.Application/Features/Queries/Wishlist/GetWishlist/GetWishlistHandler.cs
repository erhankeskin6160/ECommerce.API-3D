using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Wishlist.GetWishlist
{
    public class GetWishlistHandler : IRequestHandler<GetWishlistQuery, GetWishlistResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<GetWishlistResponse> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var items = await _wishlistRepository.GetByUserIdAsync(request.UserId);
            var result = items.Select(w => new
            {
                w.Id,
                w.ProductId,
                w.CreatedAt,
                Product = w.Product == null ? null : new
                {
                    w.Product.Id,
                    w.Product.Name,
                    w.Product.NameTr,
                    w.Product.Description,
                    w.Product.DescriptionTr,
                    w.Product.Price,
                    w.Product.ImageUrl,
                    w.Product.Category,
                    w.Product.StockQuantity
                }
            });

            return new GetWishlistResponse { Items = result };
        }
    }
}
