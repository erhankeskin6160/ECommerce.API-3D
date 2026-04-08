using MediatR;

namespace ECommerce.Application.Features.Queries.Products.GetProducts
{
    public record GetProductsQuery(string? Search, string? Category) : IRequest<GetProductsResponse>;
}
