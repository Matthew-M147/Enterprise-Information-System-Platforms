using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Tours;

public class EditModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public EditModel(TravelAgencyContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tour = await _db.Tours.FindAsync(id);
        if (tour == null)
            return NotFound();

        Tour = tour;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Tour.Id = id;
        if (await _db.Tours.AnyAsync(t => t.Name == Tour.Name && t.Id != id))
            ModelState.AddModelError("Tour.Name", "Тур з такою назвою вже існує");
        if (!ModelState.IsValid)
            return Page();

        _db.Tours.Update(Tour);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
