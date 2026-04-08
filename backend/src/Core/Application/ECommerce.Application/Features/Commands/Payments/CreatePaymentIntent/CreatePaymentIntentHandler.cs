using MediatR;
using ECommerce.Application.Abstractions;

namespace ECommerce.Application.Features.Commands.Payments.CreatePaymentIntent
{
    public class CreatePaymentIntentHandler : IRequestHandler<CreatePaymentIntentCommand, string>
    {
        private readonly IPaymentService _paymentService;
        public CreatePaymentIntentHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Task<string> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            return _paymentService.CreatePaymentIntentAsync(request.Amount);
        }
    }
}
