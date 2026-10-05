using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ToursController : Controller
{
    private readonly TravelAgencyContext _db;

    public ToursController(TravelAgencyContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var tours = await _db.Tours
            .Include(t => t.Orders)
            .OrderBy(t => t.Id)
            .AsNoTracking()
            .ToListAsync();
        return View(tours);
    }

    public IActionResult Create()
    {
        return View(new Tour { DurationDays = 1 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tour tour)
    {
        await CheckNameIsUniqueAsync(tour);
        if (!ModelState.IsValid)
            return View(tour);

        _db.Tours.Add(tour);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _db.Tours.FindAsync(id);
        if (tour == null)
            return NotFound();
        return View(tour);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tour tour)
    {
        if (id != tour.Id)
            return NotFound();

        await CheckNameIsUniqueAsync(tour);
        if (!ModelState.IsValid)
            return View(tour);

        _db.Tours.Update(tour);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var tour = await _db.Tours
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tour == null)
            return NotFound();
        return View(tour);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tour = await _db.Tours
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tour != null)
        {
            _db.Tours.Remove(tour);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CheckNameIsUniqueAsync(Tour tour)
    {
        bool exists = await _db.Tours.AnyAsync(t => t.Name == tour.Name && t.Id != tour.Id);
        if (exists)
            ModelState.AddModelError(nameof(Tour.Name), "Тур з такою назвою вже існує");
    }
}
