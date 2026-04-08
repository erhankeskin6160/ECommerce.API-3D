using MediatR;
using ECommerce.Application.Abstractions;
using ECommerce.Application.DTOs;

namespace ECommerce.Application.Features.Queries.Orders.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderByIdResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdForUserAsync(request.Id, request.UserId);
            if (order == null)
            {
                return new GetOrderByIdResponse { IsSuccess = false };
            }

            var dto = new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Ürün",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            return new GetOrderByIdResponse { IsSuccess = true, Order = dto };
        }
    }
}
