using System.ComponentModel.DataAnnotations;

namespace ecommerceapp_backendParcial2.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }   // FK hacia Product

        [Required]
        public Guid UserId { get; set; }      // FK hacia User

        [Required]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navegación
        public Product Product { get; set; }
        public User User { get; set; }
    }
}