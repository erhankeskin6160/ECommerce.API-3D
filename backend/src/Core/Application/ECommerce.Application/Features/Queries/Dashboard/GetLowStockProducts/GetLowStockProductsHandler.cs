using MediatR;
using ECommerce.Domain.Entities;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Queries.Dashboard.GetLowStockProducts
{
    public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsQuery, IEnumerable<Product>>
    {
        private readonly IDashboardRepository _repo;
        public GetLowStockProductsHandler(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetLowStockProductsAsync();
        }
    }
}
