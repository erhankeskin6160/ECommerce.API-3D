namespace ECommerce.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public required string UserId { get; set; }
        public AppUser? User { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
