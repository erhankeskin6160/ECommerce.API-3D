using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface IStockNotificationRepository
    {
        Task<IEnumerable<StockNotification>> GetByProductIdAsync(int productId);
        Task<bool> ExistsAsync(string userId, int productId);
        Task<StockNotification> AddAsync(StockNotification stockNotification);
        Task<StockNotification?> GetAsync(string userId, int productId);
        Task RemoveAsync(StockNotification stockNotification);
        Task SaveChangesAsync();
    }
}
