using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Notifications.GetUnreadCount
{
    public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, GetUnreadCountResponse>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadCountHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<GetUnreadCountResponse> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var count = await _notificationRepository.GetUnreadCountAsync(request.UserId);
            return new GetUnreadCountResponse { Count = count };
        }
    }
}
