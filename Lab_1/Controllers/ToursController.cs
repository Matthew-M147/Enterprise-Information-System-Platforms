using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/tours")]
public class ToursController : ControllerBase
{
    private static readonly Expression<Func<Tour, TourDto>> AsDto = t => new TourDto(
        t.Id, t.Name, t.Description, t.Price, t.DurationDays, t.Orders.Count);

    private readonly TravelAgencyContext _db;

    public ToursController(TravelAgencyContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<TourDto>> GetAll()
    {
        return await _db.Tours.OrderBy(t => t.Id).Select(AsDto).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TourDto>> Get(int id)
    {
        var tour = await FindDtoAsync(id);
        if (tour == null)
            return NotFound();
        return tour;
    }

    [HttpPost]
    public async Task<ActionResult<TourDto>> Create(Tour tour)
    {
        tour.Id = 0;
        await CheckNameIsUniqueAsync(tour);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Tours.Add(tour);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = tour.Id }, await FindDtoAsync(tour.Id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Tour tour)
    {
        if (!await _db.Tours.AnyAsync(t => t.Id == id))
            return NotFound();

        tour.Id = id;
        await CheckNameIsUniqueAsync(tour);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        _db.Tours.Update(tour);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tour = await _db.Tours
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tour == null)
            return NotFound();

        _db.Tours.Remove(tour);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private Task<TourDto?> FindDtoAsync(int id)
    {
        return _db.Tours.Where(t => t.Id == id).Select(AsDto).FirstOrDefaultAsync();
    }

    private async Task CheckNameIsUniqueAsync(Tour tour)
    {
        bool exists = await _db.Tours.AnyAsync(t => t.Name == tour.Name && t.Id != tour.Id);
        if (exists)
            ModelState.AddModelError(nameof(Tour.Name), "Тур з такою назвою вже існує");
    }
}
