using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ClientsController : Controller
{
    private readonly TravelAgencyContext _db;

    public ClientsController(TravelAgencyContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _db.Clients
            .Include(c => c.Orders)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
        return View(clients);
    }

    public IActionResult Create()
    {
        return View(new Client());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Client client)
    {
        await CheckPhoneIsUniqueAsync(client);
        if (!ModelState.IsValid)
            return View(client);

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return NotFound();
        return View(client);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (id != client.Id)
            return NotFound();

        await CheckPhoneIsUniqueAsync(client);
        if (!ModelState.IsValid)
            return View(client);

        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
            return NotFound();
        return View(client);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckPhoneIsUniqueAsync(Client client)
    {
        bool exists = await _db.Clients.AnyAsync(c => c.Phone == client.Phone && c.Id != client.Id);
        if (exists)
            ModelState.AddModelError(nameof(Client.Phone), "Клієнт з таким телефоном вже існує");
    }
}
