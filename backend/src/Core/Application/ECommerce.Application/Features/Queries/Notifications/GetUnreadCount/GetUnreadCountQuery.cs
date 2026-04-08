using MediatR;

namespace ECommerce.Application.Features.Queries.Notifications.GetUnreadCount
{
    public record GetUnreadCountQuery(string UserId) : IRequest<GetUnreadCountResponse>;

    public class GetUnreadCountResponse
    {
        public int Count { get; set; }
    }
}
