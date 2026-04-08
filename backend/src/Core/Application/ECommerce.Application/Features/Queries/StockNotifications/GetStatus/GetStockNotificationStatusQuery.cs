using MediatR;

namespace ECommerce.Application.Features.Queries.StockNotifications.GetStatus
{
    public class GetStockNotificationStatusQuery : IRequest<bool>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
