using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public IndexModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public List<Client> Clients { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Clients = await _db.Clients
            .Include(c => c.Orders)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
    }
}
