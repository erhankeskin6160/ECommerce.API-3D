using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.Abstractions;
using ECommerce.Infrastructure.Services;

namespace ECommerce.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
