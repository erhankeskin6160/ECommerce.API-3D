namespace ECommerce.Application.Features.Queries.Products.GetCategories
{
    public class GetCategoriesResponse
    {
        public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
    }
}
