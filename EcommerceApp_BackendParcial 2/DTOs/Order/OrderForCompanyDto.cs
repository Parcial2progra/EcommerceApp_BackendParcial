namespace ecommerceapp_backendParcial2.DTOs.Order
{
    public class OrderForCompanyDto
    {
        public Guid OrderId { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}