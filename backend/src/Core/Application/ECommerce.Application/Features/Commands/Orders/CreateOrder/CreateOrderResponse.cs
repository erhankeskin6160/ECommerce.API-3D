namespace ECommerce.Application.Features.Commands.Orders.CreateOrder
{
    public class CreateOrderResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int OrderId { get; set; }
    }
}
