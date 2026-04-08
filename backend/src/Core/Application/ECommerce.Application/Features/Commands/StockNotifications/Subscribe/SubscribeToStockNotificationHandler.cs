using MediatR;
using ECommerce.Domain.Entities;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.StockNotifications.Subscribe
{
    public class SubscribeToStockNotificationHandler : IRequestHandler<SubscribeToStockNotificationCommand, SubscribeResponse>
    {
        private readonly IProductRepository _productRepo;
        private readonly IStockNotificationRepository _stockNotifRepo;

        public SubscribeToStockNotificationHandler(IProductRepository productRepo, IStockNotificationRepository stockNotifRepo)
        {
            _productRepo = productRepo;
            _stockNotifRepo = stockNotifRepo;
        }

        public async Task<SubscribeResponse> Handle(SubscribeToStockNotificationCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null) return new SubscribeResponse { Success = false, Message = "Ürün bulunamadı." };

            if (product.StockQuantity > 0)
                return new SubscribeResponse { Success = false, Message = "Bu ürün şu anda stokta bulunmaktadır." };

            var existingSubscription = await _stockNotifRepo.GetAsync(request.UserId, request.ProductId);
            
            if (existingSubscription != null && !existingSubscription.IsNotified)
                return new SubscribeResponse { Success = false, Message = "Zaten bu ürün için stok bildirimi abonesisiniz." };

            if (existingSubscription != null && existingSubscription.IsNotified)
            {
               existingSubscription.IsNotified = false;
               await _stockNotifRepo.SaveChangesAsync();
            }
            else 
            {
               await _stockNotifRepo.AddAsync(new StockNotification
               {
                   UserId = request.UserId,
                   ProductId = request.ProductId
               });
            }

            return new SubscribeResponse { Success = true, Message = "Stok bildirimi başarıyla oluşturuldu." };
        }
    }
}
