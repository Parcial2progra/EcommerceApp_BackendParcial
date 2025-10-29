namespace ecommerceapp_backendParcial2.DTOs.Order
{
    public class OrderCreateDto
    {
        public List<OrderItemCreateDto> Items { get; set; }
        public Guid CustomerId { get; set; }
    }
}