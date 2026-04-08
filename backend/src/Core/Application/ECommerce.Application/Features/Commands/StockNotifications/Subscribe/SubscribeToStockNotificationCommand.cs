using MediatR;

namespace ECommerce.Application.Features.Commands.StockNotifications.Subscribe
{
    public class SubscribeToStockNotificationCommand : IRequest<SubscribeResponse>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class SubscribeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
