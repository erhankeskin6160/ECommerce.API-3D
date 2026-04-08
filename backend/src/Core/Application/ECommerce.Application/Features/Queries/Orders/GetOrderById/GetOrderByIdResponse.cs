using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Orders.GetOrderById
{
    public class GetOrderByIdResponse
    {
        public bool IsSuccess { get; set; }
        public OrderResponseDto? Order { get; set; }
    }
}
