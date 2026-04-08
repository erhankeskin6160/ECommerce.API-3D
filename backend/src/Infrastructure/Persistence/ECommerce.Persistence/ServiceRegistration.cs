using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ECommerce.Infrastructure.Persistence.Data;
using ECommerce.Infrastructure.Persistence.Repositories;
using ECommerce.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ECommerceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repository registrations
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IStockNotificationRepository, StockNotificationRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
        }
    }
}
