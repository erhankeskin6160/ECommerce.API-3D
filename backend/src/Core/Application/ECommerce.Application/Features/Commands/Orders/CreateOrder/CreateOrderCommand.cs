using MediatR;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Commands.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<CreateOrderResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
