using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using ECommerce.Application.Features.Queries.Notifications.GetNotifications;
using ECommerce.Application.Features.Queries.Notifications.GetUnreadCount;
using ECommerce.Application.Features.Commands.Notifications.MarkRead;
using ECommerce.Application.Features.Commands.Notifications.MarkAllRead;
using ECommerce.Domain.Entities;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetNotificationsQuery(userId));
            return Ok(response.Notifications);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetUnreadCountQuery(userId));
            return Ok(response.Count);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new MarkReadCommand { Id = id, UserId = userId });
            if (!response.IsSuccess) return NotFound();

            return NoContent();
        }

        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _mediator.Send(new MarkAllReadCommand(userId));
            return NoContent();
        }
    }
}
