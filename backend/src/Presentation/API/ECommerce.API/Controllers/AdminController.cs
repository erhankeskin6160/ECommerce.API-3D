using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.Application.Features.Queries.AdminOrders.GetAllOrders;
using ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderStatus;
using ECommerce.Application.Features.Commands.AdminOrders.UpdateOrderShipping;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(result);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand { OrderId = id, Status = dto.Status });
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(result);
        }

        [HttpPut("orders/{id}/shipping")]
        public async Task<IActionResult> UpdateOrderShipping(int id, [FromBody] UpdateShippingDto dto)
        {
            var result = await _mediator.Send(new UpdateOrderShippingCommand 
            { 
                OrderId = id, 
                ShippingCompany = dto.ShippingCompany,
                TrackingNumber = dto.TrackingNumber,
                TrackingUrl = dto.TrackingUrl
            });
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(result);
        }
    }

    public class UpdateStatusDto
    {
        public required string Status { get; set; }
    }

    public class UpdateShippingDto
    {
        public string ShippingCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string TrackingUrl { get; set; } = string.Empty;
    }
}
