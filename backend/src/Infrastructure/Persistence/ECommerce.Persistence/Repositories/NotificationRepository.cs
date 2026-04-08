using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ECommerceDbContext _context;

        public NotificationRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
            => await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

        public async Task<Notification?> GetByIdAsync(int id)
            => await _context.Notifications.FindAsync(id);

        public async Task<Notification> AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }
    }
}
