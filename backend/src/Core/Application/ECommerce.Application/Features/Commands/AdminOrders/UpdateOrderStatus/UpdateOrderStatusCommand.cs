using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<UpdateStatusResponse>
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateStatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
