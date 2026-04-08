using MediatR;

namespace ECommerce.Application.Features.Queries.Products.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<GetProductByIdResponse>;
}
