namespace ECommerce.Application.Features.Commands.ProductC.CreateProduct
{
    public class CreateProductResponse
    {
        public int ProductId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
