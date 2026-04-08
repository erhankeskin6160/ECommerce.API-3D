using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Notifications.MarkRead
{
    public class MarkReadHandler : IRequestHandler<MarkReadCommand, MarkReadResponse>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkReadHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<MarkReadResponse> Handle(MarkReadCommand request, CancellationToken cancellationToken)
        {
            var notif = await _notificationRepository.GetByIdAsync(request.Id);
            if (notif == null || notif.UserId != request.UserId)
            {
                return new MarkReadResponse { IsSuccess = false };
            }

            notif.IsRead = true;
            await _notificationRepository.UpdateAsync(notif);

            return new MarkReadResponse { IsSuccess = true };
        }
    }
}
