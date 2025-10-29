using System.ComponentModel.DataAnnotations;

namespace ecommerceapp_backendParcial2.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public Guid CompanyId { get; set; }  // FK obligatoria

        // Relaciones
        public User Company { get; set; }    // obligatorio, como en el ejemplo
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}