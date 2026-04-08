using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs
{
    public class RegisterDto
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderItemDto>? OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        [Required] public string CurrentPassword { get; set; } = string.Empty;
        [Required] public string NewPassword { get; set; } = string.Empty;
    }
}
