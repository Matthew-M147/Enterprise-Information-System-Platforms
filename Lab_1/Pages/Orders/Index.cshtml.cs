using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Orders;

public class IndexModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public IndexModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public List<Order> Orders { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Orders = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Tour)
            .OrderBy(o => o.ClientId)
            .ThenBy(o => o.TripDate)
            .AsNoTracking()
            .ToListAsync();
    }
}
