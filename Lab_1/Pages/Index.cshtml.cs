using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages;

public class IndexModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public IndexModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public int ClientsCount { get; private set; }
    public int ToursCount { get; private set; }
    public int OrdersCount { get; private set; }

    public async Task OnGetAsync()
    {
        ClientsCount = await _db.Clients.CountAsync();
        ToursCount = await _db.Tours.CountAsync();
        OrdersCount = await _db.Orders.CountAsync();
    }
}
