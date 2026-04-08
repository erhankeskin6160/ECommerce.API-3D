namespace ECommerce.Application.Abstractions
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(long amount);
    }
}
