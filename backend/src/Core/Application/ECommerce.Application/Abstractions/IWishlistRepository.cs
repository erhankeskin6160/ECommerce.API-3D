using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId);
        Task<IEnumerable<int>> GetProductIdsByUserIdAsync(string userId);
        Task<int> GetCountByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string userId, int productId);
        Task<WishlistItem> AddAsync(WishlistItem item);
        Task<WishlistItem?> GetItemAsync(string userId, int productId);
        Task RemoveAsync(WishlistItem item);
    }
}
