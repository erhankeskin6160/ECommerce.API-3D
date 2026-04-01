namespace ECommerce.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public string? NameTr { get; set; }
        public string? DescriptionTr { get; set; }
        public List<string> AdditionalImages { get; set; } = new();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
