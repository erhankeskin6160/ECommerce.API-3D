using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateStatusResponse>
    {
        private readonly IOrderRepository _repo;
        private readonly INotificationRepository _notificationRepo;

        public UpdateOrderStatusHandler(IOrderRepository repo, INotificationRepository notificationRepo)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
        }

        public async Task<UpdateStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _repo.GetOrderByIdAsync(request.OrderId);
            if (order == null) return new UpdateStatusResponse { Success = false, Message = "Sipariş bulunamadı." };

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(request.Status)) return new UpdateStatusResponse { Success = false, Message = "Geçersiz sipariş durumu." };

            order.Status = request.Status;
            await _repo.UpdateAsync(order);

            var statusMessagesTr = new Dictionary<string, string>
            {
                { "Pending", "beklemede" },
                { "Processing", "hazırlanıyor" },
                { "Shipped", "kargoya verildi" },
                { "Delivered", "teslim edildi" },
                { "Cancelled", "iptal edildi" }
            };
            
            var statusTextTr = statusMessagesTr.ContainsKey(request.Status) ? statusMessagesTr[request.Status] : request.Status;

            await _notificationRepo.AddAsync(new Notification
            {
                UserId = order.UserId,
                Title = "Sipariş Durumu Güncellendi",
                Message = $"Sipariş #{order.Id} {statusTextTr}.",
                Type = "order_status",
                OrderId = order.Id
            });

            return new UpdateStatusResponse { Success = true, Message = "Sipariş durumu güncellendi.", Status = order.Status };
        }
    }
}
