using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Data;
using ECommerce.API.Models;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockNotificationsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public StockNotificationsController(ECommerceDbContext context)
        {
            _context = context;
        }

        // POST: api/StockNotifications/{productId}/subscribe
        [HttpPost("{productId}/subscribe")]
        public async Task<IActionResult> Subscribe(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Ürün bulunamadı.");

            if (product.StockQuantity > 0)
            {
                return BadRequest("Bu ürün şu anda stokta bulunmaktadır.");
            }

            var existingSubscription = await _context.StockNotifications
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId && !s.IsNotified);

            if (existingSubscription != null)
            {
                return BadRequest("Zaten bu ürün için stok bildirimi abonesisiniz.");
            }

            var notificationRule = new StockNotification
            {
                UserId = userId,
                ProductId = productId
            };

            _context.StockNotifications.Add(notificationRule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stok bildirimi başarıyla oluşturuldu." });
        }

        // GET: api/StockNotifications/{productId}/status
        [HttpGet("{productId}/status")]
        public async Task<IActionResult> GetStatus(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var isSubscribed = await _context.StockNotifications
                .AnyAsync(s => s.UserId == userId && s.ProductId == productId && !s.IsNotified);

            return Ok(new { isSubscribed });
        }
    }
}
