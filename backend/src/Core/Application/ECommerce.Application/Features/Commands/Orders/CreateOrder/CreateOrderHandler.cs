using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Commands.Orders.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotificationRepository _notificationRepository;

        public CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository, INotificationRepository notificationRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || !request.Items.Any())
                return new CreateOrderResponse { IsSuccess = false, Message = "Sipariş en az bir ürün içermelidir." };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    totalAmount += product.Price * item.Quantity;
                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });

                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0) product.StockQuantity = 0;
                    
                    await _productRepository.UpdateAsync(product);

                    if (product.StockQuantity <= 5)
                    {
                        var adminIds = await _orderRepository.GetAdminUserIdsAsync();
                        foreach (var adminId in adminIds)
                        {
                            await _notificationRepository.AddAsync(new Notification
                            {
                                UserId = adminId,
                                Title = "Kritik Stok Uyarısı",
                                Message = $"'{product.Name}' isimli ürünün stoğu {product.StockQuantity} adet kalmıştır.",
                                TitleEn = "Low Stock Alert",
                                MessageEn = $"The product '{product.Name}' has low stock. Only {product.StockQuantity} remaining.",
                                Type = "stock_alert"
                            });
                        }
                    }
                }
            }

            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                TotalAmount = totalAmount,
                Status = "Pending",
                OrderItems = orderItems
            };

            await _orderRepository.CreateAsync(order);

            await _notificationRepository.AddAsync(new Notification
            {
                UserId = request.UserId,
                Title = "Siparişiniz Alındı",
                Message = $"Sipariş #{order.Id} başarıyla oluşturuldu. Toplam: ${order.TotalAmount:F2}",
                TitleEn = "Order Received",
                MessageEn = $"Order #{order.Id} has been placed successfully. Total: ${order.TotalAmount:F2}",
                Type = "order_created",
                OrderId = order.Id
            });

            return new CreateOrderResponse { IsSuccess = true, Message = "Siparişiniz alındı!", OrderId = order.Id };
        }
    }
}
