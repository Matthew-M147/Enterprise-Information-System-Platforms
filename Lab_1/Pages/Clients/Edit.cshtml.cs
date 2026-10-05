using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Clients;

public class EditModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public EditModel(TravelAgencyContext db)
    {
        _db = db;
    }

    [BindProperty]
    public Client Client { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return NotFound();

        Client = client;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Client.Id = id;
        if (await _db.Clients.AnyAsync(c => c.Phone == Client.Phone && c.Id != id))
            ModelState.AddModelError("Client.Phone", "Клієнт з таким телефоном вже існує");
        if (!ModelState.IsValid)
            return Page();

        _db.Clients.Update(Client);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
