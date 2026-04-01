using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerce.API.Data;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public WishlistController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/wishlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetWishlist()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var items = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new
                {
                    w.Id,
                    w.ProductId,
                    w.CreatedAt,
                    Product = w.Product == null ? null : new
                    {
                        w.Product.Id,
                        w.Product.Name,
                        w.Product.NameTr,
                        w.Product.Description,
                        w.Product.DescriptionTr,
                        w.Product.Price,
                        w.Product.ImageUrl,
                        w.Product.Category,
                        w.Product.StockQuantity
                    }
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/wishlist/count
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var count = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .CountAsync();

            return Ok(new { count });
        }

        // GET: api/wishlist/check/{productId}
        [HttpGet("check/{productId}")]
        public async Task<ActionResult<object>> CheckProduct(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var exists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            return Ok(new { isFavorited = exists });
        }

        // GET: api/wishlist/product-ids
        [HttpGet("product-ids")]
        public async Task<ActionResult<IEnumerable<int>>> GetFavoritedProductIds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var ids = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.ProductId)
                .ToListAsync();

            return Ok(ids);
        }

        // POST: api/wishlist/{productId}
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            var exists = await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            if (exists)
                return Conflict(new { message = "Bu ürün zaten favorilerde." });

            _context.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorilere eklendi." });
        }

        // DELETE: api/wishlist/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (item == null)
                return NotFound(new { message = "Ürün favorilerde bulunamadı." });

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorilerden kaldırıldı." });
        }
    }
}
