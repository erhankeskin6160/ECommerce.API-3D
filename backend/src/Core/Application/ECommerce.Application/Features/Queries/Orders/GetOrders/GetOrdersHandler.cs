using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Orders.GetOrders
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, GetOrdersResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByUserIdAsync(request.UserId);
            var dtos = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                OrderDate = o.CreatedAt,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                // Notice that in our DTO we have OrderItems but originally the API had Shipping parameters
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Ürün",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });

            return new GetOrdersResponse { Orders = dtos };
        }
    }
}
