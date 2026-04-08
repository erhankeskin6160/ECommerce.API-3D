using MediatR;

namespace ECommerce.Application.Features.Commands.Products.DeleteProduct
{
    public class DeleteProductCommand : IRequest<DeleteProductResponse>
    {
        public int Id { get; set; }
    }
}
