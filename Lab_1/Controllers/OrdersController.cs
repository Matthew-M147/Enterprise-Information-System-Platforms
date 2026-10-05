using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private static readonly Expression<Func<Order, OrderDto>> AsDto = o => new OrderDto(
        o.Id,
        o.ClientId,
        o.Client.LastName + " " + o.Client.FirstName,
        o.TourId,
        o.Tour.Name,
        o.OrderDate,
        o.TripDate,
        o.Tour.DurationDays,
        o.Quantity,
        o.Tour.Price,
        o.DiscountPercent,
        Math.Round(o.Tour.Price * o.Quantity * (1 - o.DiscountPercent / 100), 2));

    private readonly TravelAgencyContext _db;

    public OrdersController(TravelAgencyContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<OrderDto>> GetAll()
    {
        return await _db.Orders
            .OrderBy(o => o.ClientId)
            .ThenBy(o => o.TripDate)
            .Select(AsDto)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> Get(int id)
    {
        var order = await FindDtoAsync(id);
        if (order == null)
            return NotFound();
        return order;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(Order order)
    {
        order.Id = 0;
        await ValidateAsync(order);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = order.Id }, await FindDtoAsync(order.Id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Order order)
    {
        if (!await _db.Orders.AnyAsync(o => o.Id == id))
            return NotFound();

        order.Id = id;
        await ValidateAsync(order);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private Task<OrderDto?> FindDtoAsync(int id)
    {
        return _db.Orders.Where(o => o.Id == id).Select(AsDto).FirstOrDefaultAsync();
    }

    private async Task ValidateAsync(Order order)
    {
        if (!await _db.Clients.AnyAsync(c => c.Id == order.ClientId))
            ModelState.AddModelError(nameof(Order.ClientId), "Клієнта не знайдено");
        if (!await _db.Tours.AnyAsync(t => t.Id == order.TourId))
            ModelState.AddModelError(nameof(Order.TourId), "Тур не знайдено");
        if (order.TripDate < order.OrderDate)
            ModelState.AddModelError(nameof(Order.TripDate), "Дата поїздки не може бути раніше дати замовлення");
    }
}
