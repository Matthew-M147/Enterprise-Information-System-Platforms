using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public DeleteModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public Tour Tour { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tour = await FindWithOrdersAsync(id);
        if (tour == null)
            return NotFound();

        Tour = tour;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var tour = await FindWithOrdersAsync(id);
        if (tour != null)
        {
            _db.Tours.Remove(tour);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }

    private Task<Tour?> FindWithOrdersAsync(int id)
    {
        return _db.Tours.Include(t => t.Orders).FirstOrDefaultAsync(t => t.Id == id);
    }
}
