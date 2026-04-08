using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDto> GetStatsAsync();
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
    }
}
