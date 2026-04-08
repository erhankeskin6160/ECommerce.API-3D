using ECommerce.Domain.Entities;

namespace ECommerce.Application.Features.Queries.Products.GetProducts
{
    public class GetProductsResponse
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    }
}
