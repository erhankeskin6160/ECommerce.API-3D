namespace ECommerce.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? NameTr { get; set; }
    }
}
