using MediatR;

namespace ECommerce.Application.Features.Queries.AdminOrders.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<object>>
    {
    }
}
