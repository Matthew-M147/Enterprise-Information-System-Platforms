using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Orders;

public class DeleteModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public DeleteModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public Order Order { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            return NotFound();

        Order = order;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order != null)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
