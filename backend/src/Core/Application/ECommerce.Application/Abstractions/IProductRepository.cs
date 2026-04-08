using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string? search, string? category);
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByIdWithReviewsAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Review>> GetReviewsAsync(int productId);
        Task<Review> AddReviewAsync(Review review);
    }
}
