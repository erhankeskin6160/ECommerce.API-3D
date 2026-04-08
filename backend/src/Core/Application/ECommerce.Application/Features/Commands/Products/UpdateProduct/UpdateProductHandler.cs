using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Products.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IStockNotificationRepository _stockNotificationRepository;

        public UpdateProductHandler(
            IProductRepository productRepository, 
            INotificationRepository notificationRepository,
            IStockNotificationRepository stockNotificationRepository)
        {
            _productRepository = productRepository;
            _notificationRepository = notificationRepository;
            _stockNotificationRepository = stockNotificationRepository;
        }

        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                return new UpdateProductResponse { IsSuccess = false, Message = "Ürün bulunamadı." };
            }

            bool stockJustArrived = existingProduct.StockQuantity == 0 && request.StockQuantity > 0;

            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.Price = request.Price;
            existingProduct.StockQuantity = request.StockQuantity;
            existingProduct.Category = request.Category;
            existingProduct.ImageUrl = request.ImageUrl;

            await _productRepository.UpdateAsync(existingProduct);

            if (stockJustArrived)
            {
                var subscriptions = await _stockNotificationRepository.GetByProductIdAsync(existingProduct.Id);
                foreach (var sub in subscriptions)
                {
                    var notification = new Domain.Entities.Notification
                    {
                        UserId = sub.UserId,
                        Title = "Beklediğiniz Ürün Stokta!",
                        Message = $"'{existingProduct.Name}' isimli ürün tekrar stoklarımıza girmiştir. Hemen satın alabilirsiniz.",
                        TitleEn = "Product Back in Stock!",
                        MessageEn = $"The product '{existingProduct.Name}' is back in stock. You can buy it now.",
                        Type = "stock_alert"
                    };
                    await _notificationRepository.AddAsync(notification);
                    sub.IsNotified = true;
                }
                
                if (subscriptions.Any())
                {
                    await _stockNotificationRepository.SaveChangesAsync();
                }
            }

            return new UpdateProductResponse { IsSuccess = true, Message = "Ürün güncellendi." };
        }
    }
}
