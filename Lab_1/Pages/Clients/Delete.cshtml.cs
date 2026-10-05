using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab_1.Pages.Clients;

public class DeleteModel : PageModel
{
    private readonly TravelAgencyContext _db;

    public DeleteModel(TravelAgencyContext db)
    {
        _db = db;
    }

    public Client Client { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = await FindWithOrdersAsync(id);
        if (client == null)
            return NotFound();

        Client = client;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var client = await FindWithOrdersAsync(id);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }

    private Task<Client?> FindWithOrdersAsync(int id)
    {
        return _db.Clients.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
    }
}
