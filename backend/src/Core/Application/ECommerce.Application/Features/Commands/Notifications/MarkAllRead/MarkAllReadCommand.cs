using MediatR;

namespace ECommerce.Application.Features.Commands.Notifications.MarkAllRead
{
    public record MarkAllReadCommand(string UserId) : IRequest<MarkAllReadResponse>;

    public class MarkAllReadResponse
    {
        public bool IsSuccess { get; set; }
    }
}
