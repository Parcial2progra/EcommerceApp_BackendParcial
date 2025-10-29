using System.ComponentModel.DataAnnotations;

namespace ecommerceapp_backendParcial2.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Relaciones
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}