using MediatR;

namespace ECommerce.Application.Features.Commands.Payments.CreatePaymentIntent
{
    public class CreatePaymentIntentCommand : IRequest<string>
    {
        public long Amount { get; set; }
    }
}
