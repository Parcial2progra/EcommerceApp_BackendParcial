using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerceapp_backendParcial2.Data;
using ecommerceapp_backendParcial2.Models;
using ecommerceapp_backendParcial2.DTOs.Product;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Solo usuarios logueados pueden crear productos
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous] // Cualquiera puede ver productos
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Company)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Company)
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
    {
        // Obtener CompanyId desde JWT (claim NameIdentifier)
        var companyIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (companyIdClaim == null) return Unauthorized("Token inválido");

        var companyId = Guid.Parse(companyIdClaim);
        var company = await _context.Users.FindAsync(companyId);
        if (company == null) return BadRequest("Company not found.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CompanyId = companyId,
            Company = company
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductCreateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Solo la empresa propietaria puede actualizar
        var companyIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (companyIdClaim == null || product.CompanyId != Guid.Parse(companyIdClaim))
            return Forbid("No autorizado para editar este producto");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Reviews)
            .Include(p => p.OrderItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        var companyIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (companyIdClaim == null || product.CompanyId != Guid.Parse(companyIdClaim))
            return Forbid("No autorizado para eliminar este producto");

        _context.Reviews.RemoveRange(product.Reviews);
        _context.OrderItems.RemoveRange(product.OrderItems);
        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

