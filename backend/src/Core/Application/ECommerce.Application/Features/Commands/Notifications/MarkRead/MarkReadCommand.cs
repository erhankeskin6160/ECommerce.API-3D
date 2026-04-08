using MediatR;

namespace ECommerce.Application.Features.Commands.Notifications.MarkRead
{
    public class MarkReadCommand : IRequest<MarkReadResponse>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class MarkReadResponse
    {
        public bool IsSuccess { get; set; }
    }
}
