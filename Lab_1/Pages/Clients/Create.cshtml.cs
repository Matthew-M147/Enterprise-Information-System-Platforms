using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Clients;

public class CreateModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public CreateModel(TravelAgencyContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Client Client { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (await _db.Clients.AnyAsync(c => c.Phone == Client.Phone))
            ModelState.AddModelError("Client.Phone", "Клієнт з таким телефоном вже існує");
        if (!ModelState.IsValid)
            return Page();

        _db.Clients.Add(Client);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
