using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerce.API.Data;
using ECommerce.API.Models;
using ECommerce.API.DTOs;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrdersController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCompany = o.ShippingCompany,
                    TrackingNumber = o.TrackingNumber,
                    TrackingUrl = o.TrackingUrl,
                    Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product != null ? oi.Product.Name : "Ürün",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var order = await _context.Orders
                .Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();

            return Ok(new OrderResponseDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                ShippingCompany = order.ShippingCompany,
                TrackingNumber = order.TrackingNumber,
                TrackingUrl = order.TrackingUrl,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Ürün",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "Sipariş en az bir ürün içermelidir." });

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                TotalAmount = dto.Items.Sum(i => i.UnitPrice * i.Quantity),
                Status = "Pending",
                OrderItems = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _context.Orders.Add(order);

            // Sipariş alındı bildirimi oluştur
            _context.Notifications.Add(new Models.Notification
            {
                UserId = userId,
                Title = "Siparişiniz Alındı",
                Message = $"Sipariş #{order.Id} başarıyla oluşturuldu. Toplam: ${order.TotalAmount:F2}",
                TitleEn = "Order Received",
                MessageEn = $"Order #{order.Id} has been placed successfully. Total: ${order.TotalAmount:F2}",
                Type = "order_created",
                OrderId = order.Id
            });

            // Profiling Admin Roles for notification
            var adminUsers = (await _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.Name == "Admin")
                .Select(x => x.UserId)
                .ToListAsync()).Distinct();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0) product.StockQuantity = 0; // Prevent negative stock

                    // Stok Uyarı Sistemi
                    if (product.StockQuantity <= 5)
                    {
                        foreach (var adminId in adminUsers)
                        {
                            _context.Notifications.Add(new Models.Notification
                            {
                                UserId = adminId,
                                Title = "Kritik Stok Uyarısı",
                                Message = $"'{product.Name}' isimli ürünün stoğu {product.StockQuantity} adet kalmıştır.",
                                TitleEn = "Low Stock Alert",
                                MessageEn = $"The product '{product.Name}' has low stock. Only {product.StockQuantity} remaining.",
                                Type = "stock_alert"
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Bildirim mesajında doğru ID olması için güncelle
            var notification = await _context.Notifications
                .Where(n => n.UserId == userId && n.Type == "order_created")
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();
            if (notification != null && notification.OrderId == 0)
            {
                notification.OrderId = order.Id;
                notification.Message = $"Sipariş #{order.Id} başarıyla oluşturuldu. Toplam: ${order.TotalAmount:F2}";
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new { id = order.Id, message = "Siparişiniz alındı!" });
        }
    }
}
