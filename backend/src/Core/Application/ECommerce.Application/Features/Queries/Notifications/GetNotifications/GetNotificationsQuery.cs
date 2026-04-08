using MediatR;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Queries.Notifications.GetNotifications
{
    public record GetNotificationsQuery(string UserId) : IRequest<GetNotificationsResponse>;

    public class GetNotificationsResponse
    {
        public IEnumerable<Notification> Notifications { get; set; } = Enumerable.Empty<Notification>();
    }
}
