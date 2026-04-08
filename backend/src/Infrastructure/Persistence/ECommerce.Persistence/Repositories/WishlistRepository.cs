using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ECommerceDbContext _context;

        public WishlistRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WishlistItem>> GetByUserIdAsync(string userId)
            => await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<int>> GetProductIdsByUserIdAsync(string userId)
            => await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.ProductId)
                .ToListAsync();

        public async Task<int> GetCountByUserIdAsync(string userId)
            => await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .CountAsync();

        public async Task<bool> ExistsAsync(string userId, int productId)
            => await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        public async Task<WishlistItem> AddAsync(WishlistItem item)
        {
            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<WishlistItem?> GetItemAsync(string userId, int productId)
            => await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        public async Task RemoveAsync(WishlistItem item)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
