using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Data;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public AdminController(ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.UserId,
                    UserEmail = o.User != null ? o.User.Email : "Unknown",
                    UserFullName = o.User != null ? o.User.FullName : "Unknown",
                    o.Status,
                    o.TotalAmount,
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.ShippingCompany,
                    o.TrackingNumber,
                    o.TrackingUrl,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.Id,
                        oi.ProductId,
                        ProductName = oi.Product != null ? oi.Product.Name : "Silinmiş Ürün",
                        oi.Quantity,
                        oi.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

        // PUT: api/admin/orders/5/status
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı." });
            }

            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest(new { message = "Geçersiz sipariş durumu." });
            }

            order.Status = dto.Status;

            // Bildirim oluştur (TR + EN)
            var statusMessagesTr = new Dictionary<string, string>
            {
                { "Pending", "beklemede" },
                { "Processing", "hazırlanıyor" },
                { "Shipped", "kargoya verildi" },
                { "Delivered", "teslim edildi" },
                { "Cancelled", "iptal edildi" }
            };
            var statusMessagesEn = new Dictionary<string, string>
            {
                { "Pending", "is pending" },
                { "Processing", "is being prepared" },
                { "Shipped", "has been shipped" },
                { "Delivered", "has been delivered" },
                { "Cancelled", "has been cancelled" }
            };
            var statusTextTr = statusMessagesTr.ContainsKey(dto.Status) ? statusMessagesTr[dto.Status] : dto.Status;
            var statusTextEn = statusMessagesEn.ContainsKey(dto.Status) ? statusMessagesEn[dto.Status] : dto.Status;

            _context.Notifications.Add(new Notification
            {
                UserId = order.UserId,
                Title = "Sipariş Durumu Güncellendi",
                Message = $"Sipariş #{order.Id} {statusTextTr}.",
                TitleEn = "Order Status Updated",
                MessageEn = $"Order #{order.Id} {statusTextEn}.",
                Type = "order_status",
                OrderId = order.Id
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sipariş durumu güncellendi.", status = order.Status });
        }

        // PUT: api/admin/orders/5/shipping
        [HttpPut("orders/{id}/shipping")]
        public async Task<IActionResult> UpdateOrderShipping(int id, [FromBody] ECommerce.API.DTOs.UpdateShippingDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı." });
            }

            order.ShippingCompany = dto.ShippingCompany;
            order.TrackingNumber = dto.TrackingNumber;
            order.TrackingUrl = dto.TrackingUrl;
            
            // Eğer sipariş durumu Beklemede/İşleniyor ise otomatik olarak "Shipped" (Kargolandı) yapabiliriz.
            if (order.Status == "Pending" || order.Status == "Processing")
            {
                order.Status = "Shipped";
            }

            // Kargo bildirimi oluştur
            _context.Notifications.Add(new Notification
            {
                UserId = order.UserId,
                Title = "Kargo Bilgisi Güncellendi",
                Message = $"Sipariş #{order.Id} {dto.ShippingCompany} kargo ile gönderildi. Takip No: {dto.TrackingNumber}",
                TitleEn = "Shipping Info Updated",
                MessageEn = $"Order #{order.Id} shipped via {dto.ShippingCompany}. Tracking: {dto.TrackingNumber}",
                Type = "order_status",
                OrderId = order.Id
            });

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Sipariş kargo bilgileri güncellendi.", 
                shippingCompany = order.ShippingCompany,
                trackingNumber = order.TrackingNumber,
                trackingUrl = order.TrackingUrl,
                status = order.Status
            });
        }
    }

    public class UpdateStatusDto
    {
        public required string Status { get; set; }
    }
}
