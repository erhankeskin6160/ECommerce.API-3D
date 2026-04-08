using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.Data;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;

        public ProductRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? search, string? category)
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
                query = query.Where(p => p.Category == category);

            return await query.Include(p => p.Reviews).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products.FindAsync(id);

        public async Task<Product?> GetByIdWithReviewsAsync(int id)
            => await _context.Products.Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<string>> GetCategoriesAsync()
            => await _context.Products
                .Where(p => p.Category != null && p.Category != "")
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Products.AnyAsync(p => p.Id == id);

        public async Task<IEnumerable<Review>> GetReviewsAsync(int productId)
            => await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review> AddReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}
