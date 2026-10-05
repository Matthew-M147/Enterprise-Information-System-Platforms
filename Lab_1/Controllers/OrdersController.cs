using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class OrdersController : Controller
{
    private readonly TravelAgencyContext _db;

    public OrdersController(TravelAgencyContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .OrderBy(o => o.ClientId)
            .ThenBy(o => o.TripDate)
            .AsNoTracking()
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Create()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        await LoadSelectListsAsync();
        return View(new Order { OrderDate = today, TripDate = today, Quantity = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        CheckDates(order);
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return View(order);
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        await LoadSelectListsAsync();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.Id)
            return NotFound();

        CheckDates(order);
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return View(order);
        }

        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            return NotFound();
        return View(order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectListsAsync()
    {
        var clients = await _db.Clients.OrderBy(c => c.LastName).AsNoTracking().ToListAsync();
        var tours = await _db.Tours.OrderBy(t => t.Name).AsNoTracking().ToListAsync();
        ViewBag.Clients = new SelectList(clients, nameof(Client.Id), nameof(Client.FullName));
        ViewBag.Tours = new SelectList(tours, nameof(Tour.Id), nameof(Tour.Name));
    }

    private void CheckDates(Order order)
    {
        if (order.TripDate < order.OrderDate)
            ModelState.AddModelError(nameof(Order.TripDate), "Дата поїздки не може бути раніше дати замовлення");
    }
}
