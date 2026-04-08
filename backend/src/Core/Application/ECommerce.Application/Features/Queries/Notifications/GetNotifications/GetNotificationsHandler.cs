using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Notifications.GetNotifications
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, GetNotificationsResponse>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationsHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<GetNotificationsResponse> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(request.UserId);
            return new GetNotificationsResponse { Notifications = notifications };
        }
    }
}
