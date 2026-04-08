using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification> AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkAllReadAsync(string userId);
    }
}
