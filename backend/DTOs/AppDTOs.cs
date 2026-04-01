namespace ECommerce.API.DTOs
{
    public class RegisterDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderDto
    {
        public string? ShippingAddress { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        
        public string? ShippingCompany { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
    }

    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }

    public class UpdateShippingDto
    {
        public required string ShippingCompany { get; set; }
        public required string TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }
    }
}
