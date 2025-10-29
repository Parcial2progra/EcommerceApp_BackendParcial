using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerceapp_backendParcial2.Data;
using ecommerceapp_backendParcial2.Models;
using ecommerceapp_backendParcial2.DTOs.Review;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReviewsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Reviews
    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .ToListAsync();

        return Ok(reviews);
    }

    // GET: api/Reviews/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(Guid id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null) return NotFound();
        return Ok(review);
    }

    // POST: api/Reviews
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null) return BadRequest("User not found");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return BadRequest("Product not found");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            User = user,
            Product = product
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    // DELETE: api/Reviews/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
