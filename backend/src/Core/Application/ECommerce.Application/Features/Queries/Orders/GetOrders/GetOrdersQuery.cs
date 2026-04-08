using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrders
{
    public record GetOrdersQuery(string UserId) : IRequest<GetOrdersResponse>;
}
