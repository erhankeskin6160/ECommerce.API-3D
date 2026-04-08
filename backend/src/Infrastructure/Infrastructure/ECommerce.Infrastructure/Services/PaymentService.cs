using ECommerce.Application.Abstractions;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        public PaymentService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        }

        public Task<string> CreatePaymentIntentAsync(long amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Task.FromResult(paymentIntent.ClientSecret);
        }
    }
}
