using MediatR;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Queries.Dashboard.GetLowStockProducts
{
    public class GetLowStockProductsQuery : IRequest<IEnumerable<Product>>
    {
    }
}
