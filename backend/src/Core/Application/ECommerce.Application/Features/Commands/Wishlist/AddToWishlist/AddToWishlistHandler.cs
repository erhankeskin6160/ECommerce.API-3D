using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Commands.Wishlist.AddToWishlist
{
    public class AddToWishlistHandler : IRequestHandler<AddToWishlistCommand, AddToWishlistResponse>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;

        public AddToWishlistHandler(IWishlistRepository wishlistRepository, IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public async Task<AddToWishlistResponse> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            var productExists = await _productRepository.ExistsAsync(request.ProductId);
            if (!productExists)
                return new AddToWishlistResponse { IsSuccess = false, Message = "Ürün bulunamadı." };

            var exists = await _wishlistRepository.ExistsAsync(request.UserId, request.ProductId);
            if (exists)
                return new AddToWishlistResponse { IsSuccess = false, Message = "Bu ürün zaten favorilerde." };

            await _wishlistRepository.AddAsync(new WishlistItem
            {
                UserId = request.UserId,
                ProductId = request.ProductId
            });

            return new AddToWishlistResponse { IsSuccess = true, Message = "Favorilere eklendi." };
        }
    }
}
