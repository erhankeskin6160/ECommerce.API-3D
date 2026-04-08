using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.StockNotifications.GetStatus
{
    public class GetStockNotificationStatusHandler : IRequestHandler<GetStockNotificationStatusQuery, bool>
    {
        private readonly IStockNotificationRepository _stockNotifRepo;
        
        public GetStockNotificationStatusHandler(IStockNotificationRepository stockNotifRepo)
        {
            _stockNotifRepo = stockNotifRepo;
        }

        public async Task<bool> Handle(GetStockNotificationStatusQuery request, CancellationToken cancellationToken)
        {
            var existingSubscription = await _stockNotifRepo.GetAsync(request.UserId, request.ProductId);
            return existingSubscription != null && !existingSubscription.IsNotified;
        }
    }
}
