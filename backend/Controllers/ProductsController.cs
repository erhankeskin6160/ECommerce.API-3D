using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Data;
using ECommerce.API.Models;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.API.DTOs;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ECommerceDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ECommerceDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Products?search=&category=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? search, [FromQuery] string? category)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(s) ||
                    (p.Description != null && p.Description.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            return await query
                .Include(p => p.Reviews)
                .ToListAsync();
        }

        // GET: api/Products/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Products
                .Where(p => p.Category != null && p.Category != "")
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            return Ok(categories);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            bool stockJustArrived = existingProduct != null && existingProduct.StockQuantity == 0 && product.StockQuantity > 0;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                if (stockJustArrived)
                {
                    var subscriptions = await _context.StockNotifications
                        .Where(s => s.ProductId == id && !s.IsNotified)
                        .ToListAsync();

                    foreach (var sub in subscriptions)
                    {
                        var notification = new Notification
                        {
                            UserId = sub.UserId,
                            Title = "Beklediğiniz Ürün Stokta!",
                            Message = $"'{product.Name}' isimli ürün tekrar stoklarımıza girmiştir. Hemen satın alabilirsiniz.",
                            TitleEn = "Product Back in Stock!",
                            MessageEn = $"The product '{product.Name}' is back in stock. You can buy it now.",
                            Type = "stock_alert"
                        };
                        _context.Notifications.Add(notification);
                        sub.IsNotified = true;
                    }
                    if (subscriptions.Any())
                    {
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // GET: api/Products/5/reviews
        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetProductReviews(int id)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.User != null ? r.User.FullName : "Anonim",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ImageUrl = r.ImageUrl,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // POST: api/Products/5/reviews
        [Authorize]
        [HttpPost("{id}/reviews")]
        public async Task<ActionResult<ReviewDto>> PostProductReview(int id, [FromForm] CreateReviewDto reviewDto)
        {
            if (id != reviewDto.ProductId)
            {
                return BadRequest("Ürün ID uyumsuz.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var productExists = await _context.Products.AnyAsync(p => p.Id == id);
            if (!productExists) return NotFound("Ürün bulunamadı.");

            var hasPurchased = await _context.Orders
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == id) && o.Status.ToLower() == "delivered");

            if (!hasPurchased)
            {
                return BadRequest("Bu ürünü değerlendirmek için önce satın almış ve teslim almış olmanız gerekmektedir.");
            }

            string? imageUrl = null;
            if (reviewDto.Image != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "reviews");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + reviewDto.Image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await reviewDto.Image.CopyToAsync(fileStream);
                }

                imageUrl = $"/uploads/reviews/{uniqueFileName}";
            }

            var review = new Review
            {
                ProductId = id,
                UserId = userId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            var resultDto = new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = user?.FullName,
                Rating = review.Rating,
                Comment = review.Comment,
                ImageUrl = review.ImageUrl,
                CreatedAt = review.CreatedAt
            };

            return CreatedAtAction(nameof(GetProductReviews), new { id = resultDto.ProductId }, resultDto);
        }
    }
}
