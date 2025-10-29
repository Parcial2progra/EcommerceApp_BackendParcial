using ecommerceapp_backendParcial2.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }      // ✅ obligatorio
    public User Customer { get; set; }        // navegación
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}