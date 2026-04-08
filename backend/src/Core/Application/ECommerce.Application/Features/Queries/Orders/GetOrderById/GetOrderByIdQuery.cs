using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrderById
{
    public record GetOrderByIdQuery(int Id, string UserId) : IRequest<GetOrderByIdResponse>;
}
