namespace ecommerceapp_backendParcial2.DTOs.Review;
    public class ReviewCreateDto
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
