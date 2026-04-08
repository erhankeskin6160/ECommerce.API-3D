using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using ECommerce.Application.DTOs;
using ECommerce.Application.Features.Queries.Orders.GetOrders;
using ECommerce.Application.Features.Queries.Orders.GetOrderById;
using ECommerce.Application.Features.Commands.Orders.CreateOrder;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetOrdersQuery(userId));
            return Ok(response.Orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetOrderByIdQuery(id, userId));
            if (!response.IsSuccess) return NotFound();

            return Ok(response.Order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var command = new CreateOrderCommand
            {
                UserId = userId,
                Items = dto.Items,
                ShippingAddress = dto.ShippingAddress
            };

            var response = await _mediator.Send(command);
            if (!response.IsSuccess)
                return BadRequest(new { message = response.Message });

            return CreatedAtAction(nameof(GetOrder), new { id = response.OrderId }, new { id = response.OrderId, message = response.Message });
        }
    }
}
