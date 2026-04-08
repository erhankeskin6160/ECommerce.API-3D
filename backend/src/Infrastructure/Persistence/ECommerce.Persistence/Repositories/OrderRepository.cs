using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _context;

        public OrderRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
            => await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<Order?> GetByIdForUserAsync(int id, string userId)
            => await _context.Orders
                .Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetAdminUserIdsAsync()
        {
            var adminIds = await _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == "Admin")
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync();
            return adminIds;
        }

        public async Task<bool> HasPurchasedProductAsync(string userId, int productId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && 
                               o.Status.ToLower() == "delivered" && 
                               o.OrderItems.Any(oi => oi.ProductId == productId));
        }
    }
}
