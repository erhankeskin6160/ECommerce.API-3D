using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Orders.GetOrders
{
    public class GetOrdersResponse
    {
        public IEnumerable<OrderResponseDto> Orders { get; set; } = Enumerable.Empty<OrderResponseDto>();
    }
}
