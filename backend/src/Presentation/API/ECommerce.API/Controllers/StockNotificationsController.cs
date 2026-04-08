using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using ECommerce.Application.Features.Commands.StockNotifications.Subscribe;
using ECommerce.Application.Features.Queries.StockNotifications.GetStatus;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{productId}/subscribe")]
        public async Task<IActionResult> Subscribe(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new SubscribeToStockNotificationCommand { ProductId = productId, UserId = userId });
            
            if (!result.Success) return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        [HttpGet("{productId}/status")]
        public async Task<IActionResult> GetStatus(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var isSubscribed = await _mediator.Send(new GetStockNotificationStatusQuery { ProductId = productId, UserId = userId });
            return Ok(new { isSubscribed });
        }
    }
}
