using ECommerce.Application.Abstractions;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ECommerceDbContext _context;

        public DashboardRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.AddDays(-7).Date;
            var monthStart = now.AddDays(-30).Date;

            var dailyOrders = await _context.Orders.Where(o => o.CreatedAt >= todayStart).ToListAsync();
            var weeklyOrders = await _context.Orders.Where(o => o.CreatedAt >= weekStart).ToListAsync();
            var monthlyOrders = await _context.Orders.Where(o => o.CreatedAt >= monthStart).ToListAsync();

            var random = new Random();

            return new DashboardStatsDto
            {
                DailyViews = random.Next(150, 500),
                DailyFavorites = random.Next(10, 50),
                DailyCartAdditions = random.Next(20, 80),
                DailyRevenue = dailyOrders.Sum(o => o.TotalAmount),
                DailyOrdersCount = dailyOrders.Count,
                WeeklyRevenue = weeklyOrders.Sum(o => o.TotalAmount),
                WeeklyOrdersCount = weeklyOrders.Count,
                MonthlyRevenue = monthlyOrders.Sum(o => o.TotalAmount),
                MonthlyOrdersCount = monthlyOrders.Count
            };
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .ToListAsync();
        }
    }
}
