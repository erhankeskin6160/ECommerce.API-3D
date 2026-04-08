using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderShipping
{
    public class UpdateOrderShippingCommand : IRequest<UpdateShippingResponse>
    {
        public int OrderId { get; set; }
        public string ShippingCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string TrackingUrl { get; set; } = string.Empty;
    }

    public class UpdateShippingResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ShippingCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string TrackingUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
