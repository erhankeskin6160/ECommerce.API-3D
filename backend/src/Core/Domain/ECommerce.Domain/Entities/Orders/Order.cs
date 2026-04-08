namespace ECommerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        
        // Shipping / Cargo tracking details
        public string? ShippingCompany { get; set; }
        public string? TrackingNumber { get; set; }
        public string? TrackingUrl { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
