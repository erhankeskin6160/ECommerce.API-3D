using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.Products.AddReview
{
    public class AddReviewHandler : IRequestHandler<AddReviewCommand, AddReviewResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public AddReviewHandler(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<AddReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var productExists = await _productRepository.ExistsAsync(request.ProductId);
            if (!productExists)
                return new AddReviewResponse { IsSuccess = false, Message = "Ürün bulunamadı." };

            var hasPurchased = await _orderRepository.HasPurchasedProductAsync(request.UserId, request.ProductId);
            if (!hasPurchased)
                return new AddReviewResponse { IsSuccess = false, Message = "Bu ürünü değerlendirmek için önce satın almış ve teslim almış olmanız gerekmektedir." };

            var review = new Review
            {
                ProductId = request.ProductId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                ImageUrl = request.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddReviewAsync(review);

            return new AddReviewResponse 
            { 
                IsSuccess = true, 
                Message = "Değerlendirme eklendi.",
                Review = new ReviewDto
                {
                    Id = review.Id,
                    ProductId = review.ProductId,
                    UserId = review.UserId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    ImageUrl = review.ImageUrl,
                    CreatedAt = review.CreatedAt
                }
            };
        }
    }
}
