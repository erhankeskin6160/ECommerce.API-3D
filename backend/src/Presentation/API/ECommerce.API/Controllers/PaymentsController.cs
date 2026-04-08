using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.Application.Features.Commands.Payments.CreatePaymentIntent;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult> CreatePaymentIntent([FromBody] PaymentIntentCreateRequest request)
        {
            try
            {
                var clientSecret = await _mediator.Send(new CreatePaymentIntentCommand { Amount = request.Amount });
                return Ok(new { clientSecret });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = new { message = e.Message } });
            }
        }
    }

    public class PaymentIntentCreateRequest
    {
        public long Amount { get; set; }
    }
}
