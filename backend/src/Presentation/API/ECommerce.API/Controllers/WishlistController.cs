using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using ECommerce.Application.Features.Queries.Wishlist.GetWishlist;
using ECommerce.Application.Features.Queries.Wishlist.GetWishlistCount;
using ECommerce.Application.Features.Queries.Wishlist.CheckWishlist;
using ECommerce.Application.Features.Queries.Wishlist.GetWishlistProductIds;
using ECommerce.Application.Features.Commands.Wishlist.AddToWishlist;
using ECommerce.Application.Features.Commands.Wishlist.RemoveFromWishlist;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetWishlistQuery(userId));
            return Ok(response.Items);
        }

        [HttpGet("count")]
        public async Task<ActionResult<object>> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetWishlistCountQuery(userId));
            return Ok(new { count = response.Count });
        }

        [HttpGet("check/{productId}")]
        public async Task<ActionResult<object>> CheckProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new CheckWishlistQuery(userId, productId));
            return Ok(response);
        }

        [HttpGet("product-ids")]
        public async Task<ActionResult<IEnumerable<int>>> GetFavoritedProductIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetWishlistProductIdsQuery(userId));
            return Ok(response.ProductIds);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new AddToWishlistCommand { ProductId = productId, UserId = userId });
            if (!response.IsSuccess) return Conflict(new { message = response.Message });

            return Ok(new { message = response.Message });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new RemoveFromWishlistCommand { ProductId = productId, UserId = userId });
            if (!response.IsSuccess) return NotFound(new { message = response.Message });

            return Ok(new { message = response.Message });
        }
    }
}
