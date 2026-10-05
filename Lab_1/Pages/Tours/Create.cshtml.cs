using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Tours;

public class CreateModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public CreateModel(TravelAgencyContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Tour Tour { get; set; } = new() { DurationDays = 1 };

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Tours.AnyAsync(t => t.Name == Tour.Name))
            ModelState.AddModelError("Tour.Name", "Тур з такою назвою вже існує");
        if (!ModelState.IsValid)
            return Page();

        _db.Tours.Add(Tour);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
