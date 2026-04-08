using MediatR;
using ECommerce.Application.Abstractions;
using System.Linq;

namespace ECommerce.Application.Features.Queries.AdminOrders.GetAllOrders
{
    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<object>>
    {
        private readonly IOrderRepository _repo;
        public GetAllOrdersHandler(IOrderRepository repo) { _repo = repo; }
        
        public async Task<IEnumerable<object>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repo.GetAllOrdersAsync();
            return orders.Select(o => new 
            {
                o.Id,
                o.UserId,
                UserEmail = o.User != null ? o.User.Email : "Unknown",
                UserFullName = o.User != null ? o.User.FullName : "Unknown",
                o.Status,
                o.TotalAmount,
                o.CreatedAt,
                o.ShippingAddress,
                o.ShippingCompany,
                o.TrackingNumber,
                o.TrackingUrl,
                Items = o.OrderItems.Select(oi => new {
                    oi.Id,
                    oi.ProductId,
                    ProductName = oi.Product != null ? oi.Product.Name : "Silinmiş Ürün",
                    oi.Quantity,
                    oi.UnitPrice
                })
            });
        }
    }
}
