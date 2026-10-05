using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Tours;

public class IndexModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public IndexModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public List<Tour> Tours { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Tours = await _db.Tours
            .Include(t => t.Orders)
            .OrderBy(t => t.Id)
            .AsNoTracking()
            .ToListAsync();
    }
}
