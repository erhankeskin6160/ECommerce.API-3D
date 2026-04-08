using MediatR;

namespace ECommerce.Application.Features.Queries.Products.GetCategories
{
    public record GetCategoriesQuery() : IRequest<GetCategoriesResponse>;
}
