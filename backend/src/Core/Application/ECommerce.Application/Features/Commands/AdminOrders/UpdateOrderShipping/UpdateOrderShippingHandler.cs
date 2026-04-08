using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderShipping
{
    public class UpdateOrderShippingHandler : IRequestHandler<UpdateOrderShippingCommand, UpdateShippingResponse>
    {
        private readonly IOrderRepository _repo;
        private readonly INotificationRepository _notificationRepo;

        public UpdateOrderShippingHandler(IOrderRepository repo, INotificationRepository notificationRepo)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
        }

        public async Task<UpdateShippingResponse> Handle(UpdateOrderShippingCommand request, CancellationToken cancellationToken)
        {
            var order = await _repo.GetOrderByIdAsync(request.OrderId);
            if (order == null) return new UpdateShippingResponse { Success = false, Message = "Sipariş bulunamadı." };

            order.ShippingCompany = request.ShippingCompany;
            order.TrackingNumber = request.TrackingNumber;
            order.TrackingUrl = request.TrackingUrl;

            if (order.Status == "Pending" || order.Status == "Processing")
            {
                order.Status = "Shipped";
            }

            await _repo.UpdateAsync(order);

            await _notificationRepo.AddAsync(new Notification
            {
                UserId = order.UserId,
                Title = "Kargo Bilgisi Güncellendi",
                Message = $"Sipariş #{order.Id} {request.ShippingCompany} kargo ile gönderildi. Takip No: {request.TrackingNumber}",
                Type = "order_status",
                OrderId = order.Id
            });

            return new UpdateShippingResponse 
            { 
                Success = true, 
                Message = "Sipariş kargo bilgileri güncellendi.",
                ShippingCompany = order.ShippingCompany,
                TrackingNumber = order.TrackingNumber,
                TrackingUrl = order.TrackingUrl,
                Status = order.Status
            };
        }
    }
}
