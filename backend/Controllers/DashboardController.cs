using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public DashboardController(ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.AddDays(-7).Date;
            var monthStart = now.AddDays(-30).Date;

            // Real Order Statistics
            var dailyOrders = await _context.Orders
                .Where(o => o.CreatedAt >= todayStart)
                .ToListAsync();
            
            var weeklyOrders = await _context.Orders
                .Where(o => o.CreatedAt >= weekStart)
                .ToListAsync();

            var monthlyOrders = await _context.Orders
                .Where(o => o.CreatedAt >= monthStart)
                .ToListAsync();

            var dailyRevenue = dailyOrders.Sum(o => o.TotalAmount);
            var dailyCount = dailyOrders.Count;

            var weeklyRevenue = weeklyOrders.Sum(o => o.TotalAmount);
            var weeklyCount = weeklyOrders.Count;

            var monthlyRevenue = monthlyOrders.Sum(o => o.TotalAmount);
            var monthlyCount = monthlyOrders.Count;

            // Mock Data for Views, Favorites, and Carts
            // Since we don't track page views, favorites or cart additions in the DB yet, we provide realistic dummy data.
            var random = new Random();
            var dailyViews = random.Next(150, 500);
            var dailyFavorites = random.Next(10, 50);
            var dailyCartAdditions = random.Next(20, 80);

            var result = new
            {
                dailyViews,
                dailyFavorites,
                dailyCartAdditions,
                dailyRevenue,
                dailyOrdersCount = dailyCount,
                weeklyRevenue,
                weeklyOrdersCount = weeklyCount,
                monthlyRevenue,
                monthlyOrdersCount = monthlyCount
            };

            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.StockQuantity <= 5)
                .OrderBy(p => p.StockQuantity)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.StockQuantity,
                    p.ImageUrl,
                    p.Price
                })
                .Take(10)
                .ToListAsync();

            return Ok(lowStockProducts);
        }
    }
}
