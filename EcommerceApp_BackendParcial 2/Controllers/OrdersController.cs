using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ecommerceapp_backendParcial2.Data;
using ecommerceapp_backendParcial2.Models;
using ecommerceapp_backendParcial2.DTOs.Order;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();

        return Ok(orders);
    }

    // GET: api/Orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();
        return Ok(order);
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
    {
        var customer = await _context.Users.FindAsync(dto.CustomerId);
        if (customer == null) return BadRequest("Customer not found.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = dto.CustomerId,
            Customer = customer,
            CreatedAt = DateTime.UtcNow,
            TotalPrice = 0m
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    // DELETE: api/Orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Orders/{orderId}/items
    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddOrderItem(Guid orderId, [FromBody] OrderItemCreateDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return NotFound("Order not found");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return BadRequest("Product not found");

        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            Price = product.Price * dto.Quantity
        };

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        // Recalcular TotalPrice
        order.TotalPrice = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .SumAsync(oi => oi.Price);

        await _context.SaveChangesAsync();

        return Ok(orderItem);
    }

    // DELETE: api/Orders/{orderId}/items/{itemId}
    [HttpDelete("{orderId}/items/{itemId}")]
    public async Task<IActionResult> DeleteOrderItem(Guid orderId, Guid itemId)
    {
        var orderItem = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.Id == itemId && oi.OrderId == orderId);
        if (orderItem == null) return NotFound();

        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();

        // Recalcular TotalPrice
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order != null)
        {
            order.TotalPrice = order.OrderItems.Sum(oi => oi.Price);
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }
}
