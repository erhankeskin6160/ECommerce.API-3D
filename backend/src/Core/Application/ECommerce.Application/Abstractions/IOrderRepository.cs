using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdForUserAsync(int id, string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IEnumerable<string>> GetAdminUserIdsAsync();
        Task<bool> HasPurchasedProductAsync(string userId, int productId);
    }
}
