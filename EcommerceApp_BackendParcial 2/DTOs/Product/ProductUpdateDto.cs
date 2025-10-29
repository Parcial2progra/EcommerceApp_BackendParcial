public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }   // NO nullable
    public int Stock { get; set; }       // NO nullable
    public Guid CompanyId { get; set; }  // NO nullable
}