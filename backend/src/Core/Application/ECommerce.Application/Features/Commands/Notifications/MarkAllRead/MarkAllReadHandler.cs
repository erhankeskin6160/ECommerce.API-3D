using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Notifications.MarkAllRead
{
    public class MarkAllReadHandler : IRequestHandler<MarkAllReadCommand, MarkAllReadResponse>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAllReadHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<MarkAllReadResponse> Handle(MarkAllReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAllReadAsync(request.UserId);
            return new MarkAllReadResponse { IsSuccess = true };
        }
    }
}
