using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class StockNotificationRepository : IStockNotificationRepository
    {
        private readonly ECommerceDbContext _context;

        public StockNotificationRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockNotification>> GetByProductIdAsync(int productId)
            => await _context.StockNotifications
                .Where(s => s.ProductId == productId && !s.IsNotified)
                .ToListAsync();

        public async Task<bool> ExistsAsync(string userId, int productId)
            => await _context.StockNotifications
                .AnyAsync(s => s.UserId == userId && s.ProductId == productId);

        public async Task<StockNotification> AddAsync(StockNotification stockNotification)
        {
            _context.StockNotifications.Add(stockNotification);
            await _context.SaveChangesAsync();
            return stockNotification;
        }

        public async Task<StockNotification?> GetAsync(string userId, int productId)
            => await _context.StockNotifications
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId);

        public async Task RemoveAsync(StockNotification stockNotification)
        {
            _context.StockNotifications.Remove(stockNotification);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
