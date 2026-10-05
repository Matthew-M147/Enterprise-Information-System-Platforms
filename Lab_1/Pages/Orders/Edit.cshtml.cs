using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Orders;

public class EditModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public EditModel(TravelAgencyContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Order Order { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        Order = order;
        await LoadSelectListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Order.Id = id;
        if (Order.TripDate < Order.OrderDate)
            ModelState.AddModelError("Order.TripDate", "Дата поїздки не може бути раніше дати замовлення");
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        _db.Orders.Update(Order);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var clients = await _db.Clients.OrderBy(c => c.LastName).AsNoTracking().ToListAsync();
        var tours = await _db.Tours.OrderBy(t => t.Name).AsNoTracking().ToListAsync();
        ViewData["Clients"] = new SelectList(clients, nameof(Client.Id), nameof(Client.FullName));
        ViewData["Tours"] = new SelectList(tours, nameof(Tour.Id), nameof(Tour.Name));
    }
}
