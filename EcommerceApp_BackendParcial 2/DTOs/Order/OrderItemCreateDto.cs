namespace ecommerceapp_backendParcial2.DTOs.Order
{
    public class OrderItemCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}